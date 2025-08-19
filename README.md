# Packages
Packages Used in Projects






```python
import pyodbc
import pandas as pd

# Connection parameters
server = "your-server.database.windows.net"
database = "your-database"
table = "your-schema.your-table"

# Connection string using Azure Active Directory Interactive Authentication
conn_str = (
    f"Driver={{ODBC Driver 18 for SQL Server}};"
    f"Server={server};"
    f"Database={database};"
    f"Authentication=ActiveDirectoryInteractive;"
)

# Establish connection (this will prompt for MFA via a popup)
conn = pyodbc.connect(conn_str)

# Read table into pandas dataframe
query = f"SELECT TOP 10 * FROM {table};"
df = pd.read_sql(query, conn)

print(df.head())

conn.close()
```