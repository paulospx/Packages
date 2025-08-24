import pandas as pd

print("This is a placeholder for the fix_hierarchy.py script.")


csv_file = "data.csv"
df = pd.read_csv(csv_file)

df['result'] = df['result'].bfill()
df.iloc[:, :-1] = df.iloc[:, :-1].ffill()

# 
# for index, row in df.iterrows():
#     print(index, row['result'])

# iterate from the end row to the beginning
# for index, row in df[::-1].iterrows():
#     print(index, row['result'])

# New DF
new_df = pd.DataFrame(columns=df.columns)

for index, row in df.iterrows():
    new_rows = row['result'].split('-')
    for new_row in new_rows:
        dup_row = row[:-1]
        dup_row['result'] = new_row
        new_df = pd.concat([new_df, dup_row.to_frame().T], ignore_index=True)

print("----")
print(new_df)

new_df.to_csv("new_data.csv", index=False)