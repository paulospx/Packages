import time
import os
import re
import pandas as pd
import numpy as np

# Enable future behavior for silent downcasting
pd.set_option('future.no_silent_downcasting', True)

print("Creating hierarchy files...")

last_col_name = 'Account_Range'

def append_new_to_filename(file_path):
    """Generate a new filename by appending '_new' before the file extension."""
    directory, filename = os.path.split(file_path)
    name, ext = os.path.splitext(filename)
    new_filename = f"{name}_new{ext}"
    return os.path.join(directory, new_filename), name


def apply_results(res_values, from_value, to_value, df):
    """Apply a list of values to a range of rows in the DataFrame, with progress and optimized performance."""
    
    print("Processing results...")
    total_rows = to_value - from_value
    total_new_rows = total_rows * len(res_values)

    if total_new_rows > 1_000_000:
        print(f"WARNING: Processing from {from_value} to {to_value} with {len(res_values)} new values will create {total_new_rows} new rows.")
        print(f"WARNING: You are about to create {total_new_rows} new rows. This may take a while and consume significant memory.")

    print(f"From: {from_value} to: {to_value}. Total new rows: {total_new_rows}.")
    
    # Preallocate numpy array for speeda
    base_rows = df.iloc[from_value:to_value].to_numpy(copy=True)
    col_idx = df.columns.get_loc(last_col_name)
    result_rows = []
    start_time = time.time()
    
    for i, base_row in enumerate(base_rows):
        for j, val in enumerate(res_values):
            new_row = base_row.copy()
            new_row[col_idx] = val
            result_rows.append(new_row)
        # Progress every 10%
        if total_rows > 0 and (i + 1) % max(1, total_rows // 10) == 0:
            elapsed = time.time() - start_time
            percent = int((i + 1) / total_rows * 100)
            print(f"Progress: {percent}% ({i + 1}/{total_rows} rows), elapsed: {elapsed:.2f}s")
    
    # Create DataFrame from numpy array for speed
    result_df = pd.DataFrame(result_rows, columns=df.columns)
    return result_df


def get_prefixed_range(from_int, to_int, prefix):
    print(f"Getting {to_int-from_int} elements. prefixed range {from_int} to {to_int} prefix:{prefix}.")
    """Generate a list of prefixed values from a numeric range."""
    step = 1 if from_int <= to_int else -1
    if (step > 0):
        result = [prefix + str(i) for i in np.arange(from_int, to_int + 1, 1)]
    else:
        result = [prefix + str(i) for i in np.arange(to_int , from_int + 1, 1)]
    print(f"Result Length: {len(result)}.")
    print(result[:10]) # Print first 10 elements for verification
    return result


def get_total_rows(text):
    """Get the total number of rows for a given range string with a - splitting from and to integers."""
    try:
        if text is np.nan or text is None or text == '':
            return 0
        text1 = text.replace("[", "").replace("]", "").replace(" ", "")
        print(f"Filtered text for total rows: {text1}.")
        if '-' in text1:
            start, end = text1.split('-')
            start_int = int(re.sub(r'^[A-Za-z]+', '', start))
            end_int = int(re.sub(r'^[A-Za-z]+', '', end))
            total = abs(end_int - start_int) + 1
            print(f"Total rows for range {text}: {total}.")
            return total
        else:
            return 1

    except Exception:
        print(f"ERROR: processing text: {text}")
        return 0    

    return 0

def get_range(text):
    """Parse a range string and return a list of values."""
    try:
        text = text.replace("[", "").replace("]", "").replace(" ", "")
    except Exception:
        print(f"ERROR: processing text: {text}")
        return []

    print(f"Getting range for filtered: {text}.")

    if '-' in text:
        start, end = text.split('-')
        try:
            start_int = int(re.sub(r'^[A-Za-z]+', '', start))
            end_int = int(re.sub(r'^[A-Za-z]+', '', end))
            match = re.match(r'^[A-Za-z]+', start)
            prefix_start = match.group(0) if match else ''
            if start[0].isalpha() and end[0].isalpha():
                return get_prefixed_range(start_int, end_int, prefix_start)
            else:
                return get_prefixed_range(start_int,end_int,'')
        except Exception:
            print(f"Error converting text: {text}.")

        return [start, end]
    else:
        return [text]


def get_last_first_value_not_null(df, col_name, row_nr):
    """Get the last non-null value in a column before a given row index."""
    for i in range(0, row_nr):
        if col_name in df.columns and pd.notna(df.at[i, col_name]):
            return df.at[i, col_name]
    return None

def fill_columns(df, index, level):
    """Fill missing hierarchy level values based on previous non-null entries."""
    row = df.iloc[index]
    for i in range(1, level + 1):
        col_name = f"Level_{i}"
        if pd.isna(row[col_name]):
            fix_value = get_last_first_value_not_null(df, col_name, index)
            df.at[index, col_name] = fix_value


def calculate_hierarchy(csv_file_path, last_col_name):
    """Main function to calculate and fix hierarchy from a CSV file."""
    print(f"Calculate Hierarchy for {csv_file_path}.")
    df = pd.read_csv(csv_file_path, skip_blank_lines=True)
    df["Level_Nr"] = df["Level_Nr"].astype("Int64")

    for index, row in df.iterrows():
        if pd.notna(row["Level_Nr"]):
            fill_columns(df, index, row["Level_Nr"])

    new_df = pd.DataFrame(columns=df.columns)
    keysx = [-1]
    for i, row in df.iterrows():
        if not pd.isna(row[last_col_name]):
            keysx.append(i)

    for x in range(len(keysx) - 1):
        df.iloc[keysx[x] + 1:keysx[x + 1], :-1] = df.iloc[keysx[x] + 1:keysx[x + 1], :-1].ffill()

    keysn = [x + 1 for x in keysx]
    keysn.reverse()


    # Add new column to df with number of new rows to be created
    df["new_rows"] = 0

    # If Account_Range is not null and does not contain a range, set new_rows to 1 else set to number of elements in range
    for i, row in df.iterrows():
        # update te row in the dataframe
        if pd.notna(row['Account_Range']):
            if '-' not in row['Account_Range']:
                df.at[i, "new_rows"] = 1
            else:
                df.at[i, "new_rows"] = get_total_rows(row['Account_Range'])


    df.to_csv("INTERMEDIATE.csv", index=False)

    # print a summary of new_rows including total number of new rows to be created
    total_new_rows = df["new_rows"].sum()
    print(f"Total new rows to be created: {total_new_rows}")

    print(df.head(20))

    exit(0)



    # 
    # Denormalize the ranges in the last column
    #
    results = []
    for x in range(len(keysn) - 1):
        if (keysn[x] - keysn[x + 1]) == 1:
            last_column = df.at[keysn[x + 1], last_col_name]
            results.extend(get_range(last_column))
        else:
            if x > 0:
                last_column = df.at[keysn[x] - 1, last_col_name]
                results.extend(get_range(last_column))

            print(f"Applying results for rows {keysn[x + 1]} to {keysn[x]} with {len(results)} new entries.")

            # aux_result = apply_results(results, keysn[x + 1], keysn[x], df)

            # Show the first few rows of aux_result for verification
            # new_df = pd.concat([new_df, aux_result], ignore_index=True)
            results = []

    new_csv_path, filename = append_new_to_filename(csv_file_path)
    # new_df["Filename"] = filename
    new_df.to_csv(new_csv_path, index=False)


def list_csv_last_parts(directory='.'):
    """Process all CSV files in the given directory and its subdirectories."""
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith('PS_TREE_ACCOUNT_AC_RAP_ACC_AEGONNL_AEGON_01_01_2015.csv'):
                print(f"Processing file: {file}")
                calculate_hierarchy(os.path.join(root, file), last_col_name)


# Start processing hierarchy files from the specified directory
print("...")
list_csv_last_parts(".")
