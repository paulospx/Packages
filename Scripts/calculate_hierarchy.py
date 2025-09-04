import pandas as pd

pd.set_option('future.no_silent_downcasting', True)

print("Fixing hierarchy...")

last_col_name = "BU_Range"
csv_file = "PS_TREE_BU_AC_RAP_BUN_AEGONNL_AEGON_01_01_2022.csv"
df = pd.read_csv(csv_file)
new_df = pd.DataFrame(columns=df.columns)

keysx = [ -1 ]
i = 0
for index, row in df.iterrows():
    if not pd.isna(row[last_col_name]):
        keysx.append(i)
    i = i + 1

for x in range(len(keysx)-1):
    df.iloc[keysx[x]+1:keysx[x+1], :-1] = df.iloc[keysx[x]+1:keysx[x+1], :-1].ffill()

keysn = [x + 1 for x in keysx]

# Set row as nullable integer
df["Level_Nr"] = df["Level_Nr"].astype("Int64")

df["Level_1"] = df["Level_1"].ffill()
# df["Node_ID"] = df["Node_ID"].ffill()
# df["Node_Description"] = df["Node_Description"].ffill()

levels = [ "Level_2", "Level_3", "Level_4", "Level_5", "Level_6", "Level_7", "Level_8", "Level_9", "Level_10", "Level_11", "Level_12", "Level_13" ]



def get_last_first_value_not_null(df, col_name, row_nr):
    print(f"Getting first row value not null for {col_name} @ {row_nr}")
    for i in range(0, row_nr):
        if col_name in df.columns and pd.notna(df.at[i, col_name]):
            return df.at[i, col_name]
    return None

def fill_columns(df, index, level):
    # print 2nd row df
    row = df.iloc[index]
    for i in range(2, level + 1):
        col_name = f"Level_{i}"
        if pd.isna(row[col_name]):
            fix_value = get_last_first_value_not_null(df, col_name, index)
            df.at[index, col_name] = fix_value
            print(f"Fixing {col_name} @ {i} with {fix_value} .")

# iterate through df rows if Level_Nr is not empty call funtion fill_columns()
for index, row in df.iterrows():
    if pd.notna(row["Level_Nr"]):
        fill_columns(df, index, row["Level_Nr"])

print(df.head(30))

df.to_csv("intermediate_001.csv", index=False)














keysn.reverse()

def get_range(text):
    try:
        text = text.replace("[", "").replace("]", "")
    except Exception as e:
        return []

    if '-' in text:
        start, end = text.split('-')
        if start[0].isalpha() and end[0].isalpha():
            prefix = start[0]
            start_num = int(start[1:])
            end_num = int(end[1:]) + 1
            return [f"{prefix}{i:02d}" for i in range(start_num, end_num)]
        return [start, end]
    else:
        return [text]


# create a new df with same columns as df
def apply_results(res_values, from_value, to_value):
    res = pd.DataFrame(columns=df.columns)
    for i in range(from_value, to_value-1):
        for val in res_values:
            new_row = df.iloc[i].copy()
            new_row[last_col_name] = val
            res = pd.concat([res, new_row.to_frame().T], ignore_index=True)
    return res


# iterate keysn
results = []
for x in range(len(keysn)-1):
    if (keysn[x] - keysn[x+1]) == 1:       
        c = df.at[keysn[x+1], last_col_name]
        results.extend(get_range(c))
    else:
        if (x > 0):
            c = df.at[keysn[x]-1, last_col_name]
            results.extend(get_range(c))        
        res1 = apply_results(results, keysn[x+1], keysn[x])

        new_df = pd.concat([new_df, res1], ignore_index=True)
        results = []

# Add column filename with the name of the original CSV file
new_df['filename'] = csv_file

# Result information
new_df.to_csv("new_data.csv", index=False)

# print(new_df)

