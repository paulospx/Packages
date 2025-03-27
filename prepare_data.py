# Generate a method to read an Excel file into a pandas DataFrame
import pandas as pd

def read_excel_to_dataframe(file_path, sheet_name=0):
	"""
	Reads an Excel file into a pandas DataFrame.

	Parameters:
		file_path (str): The path to the Excel file.
		sheet_name (str or int, optional): The sheet name or index to read. Defaults to the first sheet (0).

	Returns:
		pd.DataFrame: The data from the Excel file as a pandas DataFrame.
	"""
	try:
		df = pd.read_excel(file_path, sheet_name=sheet_name)
		return df
	except Exception as e:
		print(f"An error occurred while reading the Excel file: {e}")
		return None


# Generate a method to filter all column names containing a specific string and also the maturity columns
def filter_columns_containing_string_and_maturity(df, string):
    """
    Filters columns in a DataFrame that contain a specific string and also the maturity columns.

    Parameters:
        df (pd.DataFrame): The DataFrame to filter.
        string (str): The string to search for in the column names.

    Returns:
        pd.DataFrame: A DataFrame with only the columns containing the specified string and maturity columns.
    """
    if df is not None:
        filtered_columns = [col for col in df.columns if string in col or 'maturity' in col.lower()]
        return df[filtered_columns]
    else:
        print("The DataFrame is None.")
        return None
   

# How to select only the n first rows from a dataframe
def select_n_rows(df, n):
    """
    Selects the first n rows from a DataFrame.

    Parameters:
        df (pd.DataFrame): The DataFrame to select rows from.
        n (int): The number of rows to select.

    Returns:
        pd.DataFrame: A DataFrame with only the first n rows.
    """
    return df.head(n) if df is not None else None 
  

# read excel file1.xlsx and file2.xlsx into dataframes and filter columns containing 'A' and join the dataframes
def main():
    file1_path = 'file1.xlsx'
    file2_path = 'file2.xlsx'

    # Read the Excel files into DataFrames
    df1 = read_excel_to_dataframe(file1_path)
    df2 = read_excel_to_dataframe(file2_path)

    # Filter columns containing 'A'
    filtered_df1 = filter_columns_containing_string_and_maturity(df1, 'A')
    filtered_df2 = filter_columns_containing_string_and_maturity(df2, 'A')

    # Join the DataFrames on the common columns
    if filtered_df1 is not None and filtered_df2 is not None:
        merged_df = pd.merge(filtered_df1, filtered_df2, on='maturity', how='inner')
        print(merged_df)
    else:
        print("One of the DataFrames is None.")