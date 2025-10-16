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



Python Example: Silent MFA with Access Token

```python
import struct
import pyodbc
import pandas as pd
from azure.identity import DefaultAzureCredential

# Azure SQL details
server = "your-server.database.windows.net"
database = "your-database"
table = "your-schema.your-table"

# 1. Acquire an access token silently
# DefaultAzureCredential tries multiple auth methods:
# - Environment variables (AZURE_CLIENT_ID, etc.)
# - Managed Identity (if running on Azure VM/Functions/Apps)
# - Azure CLI (if you've run `az login`)
credential = DefaultAzureCredential()
token = credential.get_token("https://database.windows.net/.default")

# 2. Build connection string (no username/password)
conn_str = (
    f"Driver={{ODBC Driver 18 for SQL Server}};"
    f"Server={server};"
    f"Database={database};"
    f"Encrypt=yes;"
    f"TrustServerCertificate=no;"
    f"Connection Timeout=30;"
)

# 3. Connect using the token
# pyodbc expects the token as a bytes struct
token_bytes = bytes(token.token, "utf-8")
exptoken = b""
for i in token_bytes:
    exptoken += bytes({i})
tokenstruct = struct.pack("=i", len(exptoken)) + exptoken

conn = pyodbc.connect(conn_str, attrs_before={1256: tokenstruct})

# 4. Query the table
query = f"SELECT TOP 10 * FROM {table};"
df = pd.read_sql(query, conn)

print(df.head())

conn.close()
```


# MSSQL Using MFA

```python
import pandas as pd
import pyodbc

# --- CONFIGURATION ---
csv_file = "data.csv"
server = "yourserver.database.windows.net"
database = "your_database"
table_name = "your_table"

# --- READ CSV ---
df = pd.read_csv(csv_file)

# --- CONNECT WITH MFA ---
# This will prompt an interactive Microsoft sign-in window for MFA.
connection_string = f"""
DRIVER={{ODBC Driver 18 for SQL Server}};
SERVER={server};
DATABASE={database};
Authentication=ActiveDirectoryInteractive;
Encrypt=yes;
TrustServerCertificate=no;
"""

conn = pyodbc.connect(connection_string)
cursor = conn.cursor()

# --- OPTIONAL: create table dynamically (simple example) ---
# Adjust types and logic as needed
columns = ", ".join([f"[{col}] NVARCHAR(MAX)" for col in df.columns])
create_table_sql = f"IF OBJECT_ID(N'{table_name}', N'U') IS NULL CREATE TABLE {table_name} ({columns});"
cursor.execute(create_table_sql)
conn.commit()

# --- INSERT DATA ---
for _, row in df.iterrows():
    placeholders = ", ".join(["?"] * len(row))
    insert_sql = f"INSERT INTO {table_name} ({', '.join(df.columns)}) VALUES ({placeholders})"
    cursor.execute(insert_sql, tuple(row))
conn.commit()

# --- CLEANUP ---
cursor.close()
conn.close()

print("✅ Data imported successfully with MFA authentication.")
```



