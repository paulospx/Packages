using System.Collections;
using System.Data;
using System.Reflection;
using System.Text;

namespace Triceratops.Utilities
{
    public static class ExporterManager
    {
        /// <summary>
        /// Creates a DataTable from the public properties of objects in the specified collection.
        /// </summary>
        /// <remarks>The columns in the resulting DataTable are determined by the public instance
        /// properties of the first non-null object in the collection. Subsequent objects are expected to have the same
        /// property structure; properties not present will result in null values for those columns.</remarks>
        /// <param name="list">The collection of objects to convert to rows in the DataTable. Each object's public instance properties are
        /// mapped to columns.</param>
        /// <returns>A DataTable containing one row for each object in the collection, with columns corresponding to the public
        /// properties of the object's type. If the collection is null, empty, or contains only null elements, an empty
        /// DataTable is returned.</returns>
        public static DataTable ToDataTable(IEnumerable list)
        {
            var table = new DataTable();

            if (list == null) return table;

            var enumerator = list.GetEnumerator();
            if (!enumerator.MoveNext()) return table;

            var first = enumerator.Current;
            if (first == null) return table;

            var elementType = first.GetType();
            var props = elementType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in props)
            {
                var colType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                table.Columns.Add(p.Name, colType);
            }

            AddRowFromObject(table, first, props);

            while (enumerator.MoveNext())
            {
                AddRowFromObject(table, enumerator.Current, props);
            }

            return table;
        }

        /// <summary>
        /// Creates a DataTable from a collection of objects of type T, with columns corresponding to the public
        /// properties of T.
        /// </summary>
        /// <remarks>If the input collection is empty, the returned DataTable will contain columns for the
        /// public properties of T but no rows. The method uses reflection to determine the properties of T. All
        /// property values are copied as-is; reference and value types are handled according to their default behavior
        /// in DataTable.</remarks>
        /// <typeparam name="T">The type of objects contained in the input collection.</typeparam>
        /// <param name="list">The collection of objects to convert to a DataTable. Cannot be null.</param>
        /// <returns>A DataTable containing the data from the input collection. Each row represents an object from the
        /// collection, and each column corresponds to a public property of type T.</returns>
        public static DataTable ToDataTable<T>(IEnumerable<T> list) => ToDataTable((IEnumerable)list);

        /// <summary>
        /// Converts the specified collection of objects to a CSV-formatted string using the provided delimiter and
        /// options.
        /// </summary>
        /// <remarks>Only public instance properties of the first element's type are included as columns.
        /// All rows are generated using the same property set, based on the first element. If the collection contains
        /// elements of different types, only the properties of the first element are used for all rows.</remarks>
        /// <param name="list">The collection of objects to convert to CSV. Each object's public instance properties are included as
        /// columns.</param>
        /// <param name="delimiter">The character to use as the column delimiter. Defaults to a comma (,).</param>
        /// <param name="includeHeaders">A value indicating whether to include a header row with property names. Set to <see langword="true"/> to
        /// include headers; otherwise, <see langword="false"/>.</param>
        /// <param name="newline">The string to use for line breaks between rows. Defaults to carriage return and line feed ("\r\n").</param>
        /// <returns>A string containing the CSV representation of the collection. Returns an empty string if the collection is
        /// null or contains no elements.</returns>
        public static string ToCsv(IEnumerable list, char delimiter = ',', bool includeHeaders = true, string newline = "\r\n")
        {
            if (list == null) return string.Empty;

            var sb = new StringBuilder();
            var enumerator = list.GetEnumerator();
            if (!enumerator.MoveNext()) return string.Empty;

            var first = enumerator.Current;
            if (first == null) return string.Empty;

            var elementType = first.GetType();
            var props = elementType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (includeHeaders)
            {
                sb.Append(string.Join(delimiter.ToString(), props.Select(p => EscapeCsv(p.Name, delimiter))));
                sb.Append(newline);
            }

            AddCsvRow(sb, first, props, delimiter, newline);

            while (enumerator.MoveNext())
            {
                AddCsvRow(sb, enumerator.Current, props, delimiter, newline);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a collection of objects to a CSV-formatted string using the specified delimiter and header option.
        /// </summary>
        /// <remarks>The method uses the public properties of type <typeparamref name="T"/> as columns in
        /// the CSV output. If <paramref name="includeHeaders"/> is <see langword="true"/>, the first row will contain
        /// the property names. The method does not perform any escaping for special characters within property
        /// values.</remarks>
        /// <typeparam name="T">The type of objects contained in the collection to be converted.</typeparam>
        /// <param name="list">The collection of objects to convert to CSV format. Each object's public properties will be represented as
        /// columns.</param>
        /// <param name="delimiter">The character used to separate columns in the CSV output. Defaults to ','.</param>
        /// <param name="includeHeaders">Indicates whether to include a header row with property names in the CSV output. Set to <see
        /// langword="true"/> to include headers; otherwise, <see langword="false"/>.</param>
        /// <returns>A string containing the CSV representation of the collection. Returns an empty string if the collection is
        /// empty.</returns>
        public static string ToCsv<T>(IEnumerable<T> list, char delimiter = ',', bool includeHeaders = true)
            => ToCsv((IEnumerable)list, delimiter, includeHeaders);

        /// <summary>
        /// Exports all public list or enumerable properties from the specified object to CSV format strings.
        /// </summary>
        /// <remarks>Only public instance properties that implement <see cref="IEnumerable"/> and are not
        /// of type <see cref="string"/> are exported. Each property's value is converted to a CSV string using the
        /// specified delimiter and header option.</remarks>
        /// <param name="obj">The object containing public properties to export. Each property that implements <see cref="IEnumerable"/>
        /// (excluding <see cref="string"/>) will be converted to a CSV string.</param>
        /// <param name="delimiter">The character used to separate values in the generated CSV strings. Defaults to ','.</param>
        /// <param name="includeHeaders">Indicates whether to include property headers as the first row in each CSV string. <see langword="true"/> to
        /// include headers; otherwise, <see langword="false"/>.</param>
        /// <returns>A dictionary mapping property names to their corresponding CSV string representations. If the object is null
        /// or contains no enumerable properties, the dictionary will be empty.</returns>
        public static Dictionary<string, string> ExportListsFromObjectToCsv(object obj, char delimiter = ',', bool includeHeaders = true)
        {
            var result = new Dictionary<string, string>();
            if (obj == null) return result;

            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(p => typeof(IEnumerable).IsAssignableFrom(p.PropertyType) && p.PropertyType != typeof(string));

            foreach (var p in props)
            {
                var val = p.GetValue(obj) as IEnumerable;
                if (val == null) continue;
                var csv = ToCsv(val, delimiter, includeHeaders);
                result[p.Name] = csv;
            }

            return result;
        }

        /// <summary>
        /// Extracts all public instance properties of the specified object that are collections and converts each to a
        /// DataTable.
        /// </summary>
        /// <remarks>Only properties that implement IEnumerable and are not of type string are considered.
        /// Each collection property is converted to a DataTable using the ToDataTable method. Properties with null
        /// values are skipped.</remarks>
        /// <param name="obj">The object whose public instance collection properties will be exported to DataTables. Cannot be null.</param>
        /// <returns>A dictionary mapping property names to DataTable instances, where each DataTable represents the contents of
        /// a collection property. If the object is null or has no collection properties, the dictionary will be empty.</returns>
        public static Dictionary<string, DataTable> ExportListsFromObjectToDataTables(object obj)
        {
            var result = new Dictionary<string, DataTable>();
            if (obj == null) return result;

            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(p => typeof(IEnumerable).IsAssignableFrom(p.PropertyType) && p.PropertyType != typeof(string));

            foreach (var p in props)
            {
                var val = p.GetValue(obj) as IEnumerable;
                if (val == null) continue;
                var dt = ToDataTable(val);
                result[p.Name] = dt;
            }

            return result;
        }

        /// <summary>
        /// Adds a new row to the specified <see cref="DataTable"/> using property values from the provided object.
        /// </summary>
        /// <remarks>If a property value is null, the corresponding column in the new row will be set to
        /// <see cref="DBNull.Value"/>. All properties in <paramref name="props"/> should match existing column names in
        /// <paramref name="table"/> to avoid runtime errors.</remarks>
        /// <param name="table">The <see cref="DataTable"/> to which the new row will be added. Must not be null.</param>
        /// <param name="item">The object whose property values will be used to populate the new row. Must not be null.</param>
        /// <param name="props">An array of <see cref="PropertyInfo"/> objects representing the properties to extract from <paramref
        /// name="item"/>. Each property's name must correspond to a column in <paramref name="table"/>.</param>
        private static void AddRowFromObject(DataTable table, object item, PropertyInfo[] props)
        {
            var row = table.NewRow();
            foreach (var p in props)
            {
                var val = p.GetValue(item) ?? DBNull.Value;
                row[p.Name] = val;
            }
            table.Rows.Add(row);
        }

        /// <summary>
        /// Appends a CSV-formatted row representing the specified object's property values to the provided <see
        /// cref="StringBuilder"/>.
        /// </summary>
        /// <remarks>Each property value is converted to a string and escaped as needed for CSV
        /// formatting. Null property values are written as empty fields. The order of properties in <paramref
        /// name="props"/> determines the order of fields in the output row.</remarks>
        /// <param name="sb">The <see cref="StringBuilder"/> to which the CSV row will be appended.</param>
        /// <param name="item">The object whose property values are written as a CSV row.</param>
        /// <param name="props">An array of <see cref="PropertyInfo"/> objects specifying which properties of <paramref name="item"/> to
        /// include in the CSV row.</param>
        /// <param name="delimiter">The character used to separate property values in the CSV row.</param>
        /// <param name="newline">The string to append after the CSV row to indicate a new line.</param>
        private static void AddCsvRow(StringBuilder sb, object item, PropertyInfo[] props, char delimiter, string newline)
        {
            var values = new List<string>(props.Length);
            foreach (var p in props)
            {
                var v = p.GetValue(item);
                values.Add(EscapeCsv(v?.ToString() ?? string.Empty, delimiter));
            }
            sb.Append(string.Join(delimiter.ToString(), values));
            sb.Append(newline);
        }

        /// <summary>
        /// Escapes a string for safe inclusion in a CSV field, quoting and doubling embedded quotes as needed.
        /// </summary>
        /// <remarks>Fields containing the delimiter, double quotes, or line breaks are quoted according
        /// to CSV conventions. This method does not perform full CSV row formatting; it escapes a single field
        /// value.</remarks>
        /// <param name="s">The string to be escaped for CSV output. If <paramref name="s"/> is <see langword="null"/>, an empty string
        /// is returned.</param>
        /// <param name="delimiter">The character used as the field delimiter in the CSV format. If present in the string, the field will be
        /// quoted.</param>
        /// <returns>A CSV-escaped string. If quoting is required, the result is enclosed in double quotes and embedded quotes
        /// are doubled; otherwise, the original string is returned.</returns>
        private static string EscapeCsv(string s, char delimiter)
        {
            if (s == null) return string.Empty;
            var mustQuote = s.Contains(delimiter) || s.Contains('"') || s.Contains('\r') || s.Contains('\n');
            if (mustQuote)
            {
                var escaped = s.Replace("\"", "\"\"");
                return $"\"{escaped}\"";
            }
            return s;
        }
    }
}