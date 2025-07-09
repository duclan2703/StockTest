CREATE PROCEDURE usp_RecommendCrossSellProducts
    @CustomerID INT,
    @TopN INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Step 1: Get products the customer already purchased
    SELECT DISTINCT sod.ProductID
    INTO #CustomerProducts
    FROM Sales.SalesOrderHeader soh
    JOIN Sales.SalesOrderDetail sod ON soh.SalesOrderID = sod.SalesOrderID
    WHERE soh.CustomerID = @CustomerID;

    -- Step 2: Find similar customers who bought at least 2 of same products
    SELECT soh.CustomerID
    INTO #SimilarCustomers
    FROM Sales.SalesOrderHeader soh
    JOIN Sales.SalesOrderDetail sod ON soh.SalesOrderID = sod.SalesOrderID
    WHERE sod.ProductID IN (SELECT ProductID FROM #CustomerProducts)
      AND soh.CustomerID <> @CustomerID
    GROUP BY soh.CustomerID
    HAVING COUNT(DISTINCT sod.ProductID) >= 2;

    -- Step 3: Products bought by similar customers but NOT by target customer
    SELECT sod.ProductID, COUNT(DISTINCT soh.CustomerID) AS RecommendationScore
    INTO #Recommendations
    FROM Sales.SalesOrderHeader soh
    JOIN Sales.SalesOrderDetail sod ON soh.SalesOrderID = sod.SalesOrderID
    WHERE soh.CustomerID IN (SELECT CustomerID FROM #SimilarCustomers)
      AND sod.ProductID NOT IN (SELECT ProductID FROM #CustomerProducts)
    GROUP BY sod.ProductID;

    -- Step 4: Final result with product names
    SELECT TOP (@TopN)
        r.ProductID,
        p.Name AS ProductName,
        r.RecommendationScore
    FROM #Recommendations r
    JOIN Production.Product p ON r.ProductID = p.ProductID
    ORDER BY r.RecommendationScore DESC, p.Name;

    -- Cleanup
    DROP TABLE IF EXISTS #CustomerProducts;
    DROP TABLE IF EXISTS #SimilarCustomers;
    DROP TABLE IF EXISTS #Recommendations;
END;
