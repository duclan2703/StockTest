CREATE PROCEDURE usp_AnalyzeYearlyGrowth
    @StartYear INT,
    @EndYear INT,
    @ProductCategoryID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table for intermediate data
    IF OBJECT_ID('tempdb..#RevenueByYear') IS NOT NULL
        DROP TABLE #RevenueByYear;

    SELECT
        YEAR(soh.OrderDate) AS [Year],
        SUM(soh.TotalDue) AS TotalRevenue
    INTO #RevenueByYear
    FROM Sales.SalesOrderHeader soh
    INNER JOIN Sales.SalesOrderDetail sod ON soh.SalesOrderID = sod.SalesOrderID
    INNER JOIN Production.Product p ON sod.ProductID = p.ProductID
    INNER JOIN Production.ProductSubcategory ps ON p.ProductSubcategoryID = ps.ProductSubcategoryID
    INNER JOIN Production.ProductCategory pc ON ps.ProductCategoryID = pc.ProductCategoryID
    WHERE YEAR(soh.OrderDate) BETWEEN @StartYear AND @EndYear
      AND (@ProductCategoryID IS NULL OR pc.ProductCategoryID = @ProductCategoryID)
    GROUP BY YEAR(soh.OrderDate);

    -- Final result with growth rate
    SELECT
        r.Year,
        r.TotalRevenue,
        CAST(
            CASE 
                WHEN LAG(r.TotalRevenue) OVER (ORDER BY r.Year) IS NULL THEN NULL
                WHEN LAG(r.TotalRevenue) OVER (ORDER BY r.Year) = 0 THEN NULL
                ELSE
                    ((r.TotalRevenue - LAG(r.TotalRevenue) OVER (ORDER BY r.Year)) * 100.0) 
                    / NULLIF(LAG(r.TotalRevenue) OVER (ORDER BY r.Year), 0)
            END AS DECIMAL(10, 2)
        ) AS GrowthRatePercent
    FROM #RevenueByYear r
    ORDER BY r.Year;
END;
