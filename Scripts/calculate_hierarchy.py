import pandas as pd

print("This is a placeholder for the fix_hierarchy.py script.")


csv_file = "data.csv"
df = pd.read_csv(csv_file)


# Iterate df
keysx = [ -1 ]
i = 0
for index, row in df.iterrows():
    if not pd.isna(row['result']):
        keysx.append(i)
    i = i + 1

for x in range(len(keysx)-1):
    df.iloc[keysx[x]+1:keysx[x+1]+1, :-1] = df.iloc[keysx[x]+1:keysx[x+1]+1, :-1].ffill()




def get_range(text):
    # replace [ and ] with empty string
    try:
        text = text.replace("[", "").replace("]", "")
    except Exception as e:
        # print(f"Error processing text: {e}")
        return []

    if '-' in text:
        start, end = text.split('-')
        # check if the first character is a letter i.e. A-Z
        if start[0].isalpha() and end[0].isalpha():
            prefix = start[0]
            start_num = int(start[1:])
            end_num = int(end[1:]) + 1
            return [f"{prefix}{i:02d}" for i in range(start_num, end_num)]
        return [start, end]
    else:
        return [text]



new_rows = []
for index, row in df.iterrows():
    if not pd.isna(row['result']):
        # if contains - split into and add values into new_rows
        new_rows.extend(get_range(row['result']))
    else:
        # else add the value as is
        new_rows.append(row['result'])
    i = i + 1



# New DataFrame
new_df = pd.DataFrame(columns=df.columns)
for index, row in df.iterrows():
    new_rows = get_range(row['result'])
    # while next row['result'] is not nan
    # while index + 1 < len(df) and not pd.isna(df.iloc[index + 1]['result']):
    #     new_rows.extend(get_range(df.iloc[index + 1]['result']))
    #     index += 1

    for new_row in new_rows:
        dup_row = row[:-1]
        dup_row['result'] = new_row
        new_df = pd.concat([new_df, dup_row.to_frame().T], ignore_index=True)


print("----")
print(new_df)

# new_df.to_csv("new_data.csv", index=False)