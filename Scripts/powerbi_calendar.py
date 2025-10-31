import pandas as pd

# Define start and end dates for the calendar
start_date = "2020-01-01"
end_date = "2030-12-31"

# Generate date range
dates = pd.date_range(start=start_date, end=end_date, freq="D")

# Create dataframe
calendar_df = pd.DataFrame({"Date": dates})

# Add useful columns
calendar_df["Year"] = calendar_df["Date"].dt.year
calendar_df["Month"] = calendar_df["Date"].dt.month
calendar_df["MonthName"] = calendar_df["Date"].dt.strftime("%B")
calendar_df["Quarter"] = calendar_df["Date"].dt.quarter
calendar_df["DayOfWeek"] = calendar_df["Date"].dt.weekday + 1  # Monday=1
calendar_df["DayName"] = calendar_df["Date"].dt.strftime("%A")
calendar_df["WeekOfYear"] = calendar_df["Date"].dt.isocalendar().week
calendar_df["IsWeekend"] = calendar_df["DayOfWeek"].isin([6, 7])

# Fiscal year example (starts in July)
calendar_df["FiscalYear"] = calendar_df["Date"].apply(
    lambda d: d.year + 1 if d.month >= 7 else d.year
)
calendar_df["FiscalQuarter"] = ((calendar_df["Date"].dt.month - 7) % 12) // 3 + 1

# Show the first few rows
print(calendar_df.head())