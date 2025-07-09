CREATE FUNCTION ufn_GetCustomerRepeatPurchaseBehavior
(
    @CustomerID INT
)
RETURNS @Result TABLE
(
    ProductSubcategoryID INT,
    ProductSubcategoryName NVARCHAR(100),
    OrderCount INT,
    AvgGapDays DECIMAL(10, 2)
)
AS
BEGIN
    ;WITH CustomerOrders AS (
        SELECT DISTINCT
            soh.SalesOrderID,
            soh.OrderDate,
            ps.ProductSubcategoryID,
            ps.Name AS ProductSubcategoryName
        FROM Sales.SalesOrderHeader soh
        JOIN Sales.SalesOrderDetail sod ON soh.SalesOrderID = sod.SalesOrderID
        JOIN Production.Product p ON sod.ProductID = p.ProductID
        JOIN Production.ProductSubcategory ps ON p.ProductSubcategoryID = ps.ProductSubcategoryID
        WHERE soh.CustomerID = @CustomerID
    ),
    RankedOrders AS (
        SELECT
            ProductSubcategoryID,
            ProductSubcategoryName,
            SalesOrderID,
            OrderDate,
            ROW_NUMBER() OVER (PARTITION BY ProductSubcategoryID ORDER BY OrderDate) AS rn,
            LAG(OrderDate) OVER (PARTITION BY ProductSubcategoryID ORDER BY OrderDate) AS PrevOrderDate
        FROM CustomerOrders
    ),
    Gaps AS (
        SELECT
            ProductSubcategoryID,
            ProductSubcategoryName,
            DATEDIFF(DAY, PrevOrderDate, OrderDate) AS GapDays
        FROM RankedOrders
        WHERE PrevOrderDate IS NOT NULL
    )
    INSERT INTO @Result
    SELECT
        r.ProductSubcategoryID,
        r.ProductSubcategoryName,
        COUNT(*) + 1 AS OrderCount, -- since LAG removes the first
        AVG(CAST(g.GapDays AS DECIMAL(10, 2))) AS AvgGapDays
    FROM Gaps g
    JOIN RankedOrders r ON g.ProductSubcategoryID = r.ProductSubcategoryID
    GROUP BY r.ProductSubcategoryID, r.ProductSubcategoryName
    HAVING COUNT(*) >= 1 -- implies at least 2 orders total

    RETURN;
END;