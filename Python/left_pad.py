import pandas as pd

# Assume you have two DataFrames:
# df_left: the table with the 'FROM' field (8-digit zero-padded strings)
# df_right: the table with the 'ACCOUNT_NR' field (mixed padding or integers)

# Example data (replace with your actual DataFrames or loading code)
df_left = pd.read_csv('left_table.csv')   # or pd.read_sql, etc.
df_right = pd.read_csv('right_table.csv') # or whatever source

# Step 1: Ensure both columns are strings
df_left['FROM'] = df_left['FROM'].astype(str)
df_right['ACCOUNT_NR'] = df_right['ACCOUNT_NR'].astype(str)

# Step 2: Pad both to exactly 8 digits with leading zeros
# This normalizes '12345' -> '00012345', '00123456' stays the same, '0' -> '00000000'
df_left['FROM_padded'] = df_left['FROM'].str.zfill(8)
df_right['ACCOUNT_NR_padded'] = df_right['ACCOUNT_NR'].str.zfill(8)

# Step 3: Perform the join (use how='inner' or 'left'/'right' as needed)
merged_df = pd.merge(
    df_left,
    df_right,
    left_on='FROM_padded',
    right_on='ACCOUNT_NR_padded',
    how='inner'  # change to 'left' if you want to keep all rows from df_left
)

# Step 4: Optionally drop the temporary padded columns if you don't need them
merged_df = merged_df.drop(columns=['FROM_padded', 'ACCOUNT_NR_padded'])

# Now merged_df contains the joined data
print(merged_df)

# Save result if needed
# merged_df.to_csv('joined_table.csv', index=False)