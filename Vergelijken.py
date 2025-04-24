import streamlit as st
import plotly.graph_objects as go
import pandas as pd
import os
import re

st.set_page_config(page_title="Vergelijken", page_icon="📈", layout="wide")

# Title of the Streamlit app
st.title("Vergelijken")

# Give a title an icon and make streamlit app wide by default

file1 = st.text_input("Enter the first file name (e.g., file1.csv):", "C:\\Repos\\Data\\Curves\\Book_2.xlsx")
file2 = st.text_input("Enter the second file name (e.g., file2.csv):", "C:\\Repos\\Data\\Curves\\Book_3.xlsx")
sheetname = st.selectbox("Select the sheet name:", ["AC ZC YC", "Sheet1", "Sheet2"])



# Given a path it scans for all the files with and extension and a regex pattern
def get_files(path, extension, pattern):
    files = []
    for file in os.listdir(path):
        if file.endswith(extension) and re.match(pattern, file):
            files.append(os.path.join(path, file))
    return files


# Read the file to 
df1 = pd.read_excel(file1, sheet_name=sheetname)
df2 = pd.read_excel(file2, sheet_name=sheetname)


# Plot a curve chart given a title, list of x values, and list of y values
def plot_curve_chart(title, x_values, y_values, color, ui_col):
    fig = go.Figure()
    fig.add_trace(go.Scatter(x=x_values, y=y_values, mode='lines+markers', name=title, line=dict(color=color)))
    fig.update_layout(title=title, xaxis_title='Maturity', yaxis_title='bp', showlegend=True)
    ui_col.plotly_chart(fig)

# method to plot a heatmap given a title, list of x values, and list of y values
def plot_heatmap(title, x_values, y_values, color, ui_col):
    fig = go.Figure(data=go.Heatmap(z=y_values, x=x_values, y=x_values, colorscale=color))
    fig.update_layout(title=title, xaxis_title='Curves', yaxis_title='Curves')
    ui_col.plotly_chart(fig)

# Give me a method that given a 2 dataframe, calculartes a correlation matrix and plot it as a heatmap
def plot_correlation_heatmap(df1, df2, title, color, ui_col):
    correlation_matrix = df1.corrwith(df2)
    # sort the correlation matrix by the second column
    sorted_correlation_matrix = correlation_matrix.sort_values(ascending=True)

    fig = go.Figure(data=go.Heatmap(z=sorted_correlation_matrix.values.reshape(1, -1), x=df1.columns, y=['Correlation'], colorscale=color))
    fig.update_layout(title=title, xaxis_title='Curves', yaxis_title='Correlation')
    ui_col.plotly_chart(fig)
    return sorted_correlation_matrix

sorted_corr_matrix = plot_correlation_heatmap(df1.iloc[:, 2:],df2.iloc[:, 2:], 'Correlation Matrix', 'YlGnBu', st)

col1, col2 = st.columns(2) 

# select the first column values
maturities = df1.iloc[:, 0].values.tolist()

# select the value from row 3 to row 100 from the first column dataframe
# df1 = df1.iloc[3:100, :]

# how to select the values from 3 to 100 from a list and if the list is smaller than 100, fill with nan
# df1 = df1.iloc[3:100, :].fillna(np.nan)

# add an extra column to sorted correlation matrix with categorization base on the correlation value
sorted_corr_matrix = pd.DataFrame(sorted_corr_matrix)
sorted_corr_matrix['Category'] = pd.cut(sorted_corr_matrix[0], bins=[-1.0, -0.75, -0.50, -0.25, 0.5, 0.75, 0.90, 1], labels=['Stronh Negative','Moderate Negative','Weak Negative','Neutral/Unrelated','Weak Positive', 'Moderate Positive', 'Strong Positive'])
st.table(sorted_corr_matrix)



# Print the difference between the columns names of the two frames in a summary table
st.subheader("Summary of differences between the two files")
st.write("The following columns are present in the first file but not in the second file:")
st.write(set(df1.columns) - set(df2.columns))

st.write("The following columns are present in the second file but not in the first file:")
st.write(set(df2.columns) - set(df1.columns))

st.write("The following columns are present in both files:")
st.write(set(df1.columns) & set(df2.columns))

# for all the columns with same name calculate de difference of the value and display it in a table
st.write("The following columns are present in both files:")

st.write("The following columns are present in both files:")

st.write("The following columns are present in both files:")



# ...existing code...

# Create a DataFrame to summarize differences
differences = []




def calculate_differences(df1, df2):
    # New DataFrame to store differences
    dfdelta = pd.DataFrame({"curve_name": [], "percentage": [], "abs_diff_mean": [], "cum_abs_diff": []})
    for col in df1.columns[2:]:
        if col in df2.columns:
            diff = df1[col] - df2[col]
            st.text(f"{col} {sum(abs(diff))}")	
            percentage_diff = (diff / df2[col]) * 100
            abs_diff = abs(diff)
            
            dfdelta = pd.concat([dfdelta, pd.DataFrame({"curve_name": [col], "percentage": [percentage_diff.mean()], "abs_diff_mean": [abs_diff.mean()], "cum_abs_diff": [sum(abs(diff))]})], ignore_index=True)

            # Sort by name and then by abs difference in descending order
            dfdelta = dfdelta.sort_values(by="curve_name")
            dfdelta = dfdelta.sort_values(by="cum_abs_diff", ascending=False)

    return dfdelta

delta = calculate_differences(df1, df2)

# print delta with no index

# Print description for delta
print("#Diferences between the columns present in both files")
print()
print("The following table shows the differences between the columns present in both files:")
print(f"- Source: {file1} sheet: {sheetname}")
print(f"- Target: {file2} sheet: {sheetname}")
print("The table shows the curve name, percentage difference mean, and absolute differences mean and cumulative absolute difference.")
print("The table is sorted by the cumulative absolute difference in descending order.")
print()
print(delta)

st.data_editor(delta)

# Convert the differences list to a DataFrame
differences_df = pd.DataFrame(differences)

# Display the differences in a table format
st.subheader("Differences Between Matching Columns")
st.write("The table below summarizes the differences between columns present in both files:")
st.dataframe(differences_df.style.format({
    "Mean Difference": "{:.6f}",
    "Mean Percentage Difference (%)": "{:.6f}",
    "Mean Absolute Difference": "{:.6f}"
}).highlight_max(axis=0, color="green").highlight_min(axis=0, color="red"))

# Add a note for better context
st.info("The table above shows the mean differences, percentage differences, and absolute differences for each matching column. Highlighted values indicate the maximum and minimum differences.")
        

