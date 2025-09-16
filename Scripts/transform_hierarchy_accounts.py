import time
import os
import re
import pandas as pd
import numpy as np
import logging 

logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger(__name__)

# Enable future behavior for silent downcasting
pd.set_option('future.no_silent_downcasting', True)

logger.info("Creating hierarchy files...")

last_col_name = 'Account_Range'

def get_filenames(file_path):
    """Generate a new filename by appending '_new' before the file extension."""
    directory, filename = os.path.split(file_path)
    name, ext = os.path.splitext(filename)
    new_filename = f"{name}_new{ext}"
    return os.path.join(directory, new_filename), name


# create a new df with same columns as df
def apply_results(res_values, from_value, to_value, df):
    # if to_value - from_value < 20000 then log error and return empty dataframe
    if abs(to_value - from_value) > 20000:
        logging.error(f"Range too large from {from_value} to {to_value}.")
        return pd.DataFrame(columns=df.columns)

    res = pd.DataFrame(columns=df.columns)
    for i in range(from_value, to_value-1):
        for val in res_values:
            new_row = df.iloc[i].copy()
            new_row[last_col_name] = val
            res = pd.concat([res, new_row.to_frame().T], ignore_index=True)
    return res


def get_prefixed_range(from_int, to_int, prefix):
    logging.info(f"Getting {to_int-from_int} elements. prefixed range {from_int} to {to_int} prefix:{prefix}.")
    
    if abs(from_int - to_int) > 10000:
        logging.error(f"Range too large from {from_int} to {to_int}.")
        return []

    """Generate a list of prefixed values from a numeric range."""
    step = 1 if from_int <= to_int else -1
    if (step > 0):
        result = [prefix + str(i) for i in np.arange(from_int, to_int + 1, 1)]
    else:
        result = [prefix + str(i) for i in np.arange(to_int , from_int + 1, -1)]
    # print(f"Result Length: {len(result)}.")
    # print(result[:10]) # Print first 10 elements for verification
    return result


def get_total_rows(text):
    """Get the total number of rows for a given range string with a - splitting from and to integers."""
    try:
        if text is np.nan or text is None or text == '':
            return 0
        text1 = text.replace("[", "").replace("]", "").replace(" ", "")
        logging.info(f"Filtered text for total rows: {text1}.")
        if '-' in text1:
            start, end = text1.split('-')
            start_int = int(re.sub(r'^[A-Za-z]+', '', start))
            end_int = int(re.sub(r'^[A-Za-z]+', '', end))
            total = abs(end_int - start_int) + 1
            logging.info(f"Total rows for range {text}: {total}.")
            return total
        else:
            return 1

    except Exception:
        logging.error(f"Getting total Rows for text: {text}")
        return 0    


def get_range(text):
    """Parse a range string and return a list of values."""
    try:
        text = text.replace("[", "").replace("]", "").replace(" ", "")
    except Exception:
        logging.error(f"Getting Range for text: {text}")
        return []

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
    COL_NR_ROWS = "nr_rows"
    COL_ACCOUNT_RANGE = "Account_Range"
    df[COL_NR_ROWS] = 0

    # If Account_Range is not null and does not contain a range, set nr_rows to 1 else set to number of elements in range
    for i, row in df.iterrows():
        # update te row in the dataframe
        if pd.notna(row[COL_ACCOUNT_RANGE]):
            if '-' not in row[COL_ACCOUNT_RANGE]:
                df.at[i, COL_NR_ROWS] = 1
            else:
                df.at[i, COL_NR_ROWS] = get_total_rows(row[COL_ACCOUNT_RANGE])

    # print a summary of nr_rows including total number of new rows to be created
    total_new_rows = df[COL_NR_ROWS].sum()
    logging.info(f"Total new rows to be created: {total_new_rows}")

    # Denormalize the ranges in the last column
    results = []
    for x in range(len(keysn)-1):
        if (keysn[x] - keysn[x+1]) == 1:       
            c = df.at[keysn[x+1], last_col_name]
            results.extend(get_range(c))
        else:
            if (x > 0):
                c = df.at[keysn[x]-1, last_col_name]
                results.extend(get_range(c))        
            res1 = apply_results(results, keysn[x+1], keysn[x], df)

            new_df = pd.concat([new_df, res1], ignore_index=True)
            results = []

    new_csv_path, filename = get_filenames(csv_file_path)
    new_df["filename"] = filename
    new_df.to_csv(new_csv_path, index=False)


def list_csv_last_parts(directory='.'):
    """Process all CSV files in the given directory and its subdirectories."""
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith('PS_TREE_ACCOUNT_AC_RAP_ACC_AEGONNL_AEGON_01_01_2015.csv'):
                print(f"Processing file: {file}")
                calculate_hierarchy(os.path.join(root, file), last_col_name)


# Start processing hierarchy files from the specified directory
logging.info("Starting to process hierarchy files...")
list_csv_last_parts(".")
