import pandas as pd

pd.set_option('future.no_silent_downcasting', True)

print("Fixing hierarchy...")

last_col_name = "result"
csv_file = "data.csv"
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
            print(new_row)
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

# Result information
new_df.to_csv("new_data.csv", index=False)
print(new_df)

