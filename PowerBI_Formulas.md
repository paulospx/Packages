# PowerBI



```DAX
Selected filter =
VAR _product =
IF(
	ISFILTERED( financials[Product] ),
	CONCATENATEX(
		DISTINCT( financials[Product] ),
		financials[Product],
		CHAR(10),
		financials[Product],
		ASC
	),
	"No Filter Selected”
)

VAR _country =
IF(
	ISFILTERED( financials[Country] ),
	CONCATENATEX(
		DISTINCT( financials[Country] ),
		financials[Country],
		CHAR(10),
		financials[Product],
		ASC
	),
	"No Filter Selected”
)
RETURN 
"Product Filters: " & UNICHAR(10) & _product & REPT( UNICHAR(10),2) &
"Product Filters: " & UNICHAR(10) & _country


```
