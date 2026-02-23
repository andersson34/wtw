USE InvoiceDb;
GO

CREATE OR ALTER PROCEDURE dbo.SP_InsertarFactura
    @NumeroFactura VARCHAR(50),
    @ClienteNombre VARCHAR(200),
    @ClienteEmail VARCHAR(320),
    @FechaEmision DATETIME,
    @FechaVencimiento DATETIME,
    @Subtotal DECIMAL(18,2),
    @Impuesto DECIMAL(18,2),
    @Total DECIMAL(18,2),
    @Estado VARCHAR(20),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Facturas
    (
        NumeroFactura,
        ClienteNombre,
        ClienteEmail,
        FechaEmision,
        FechaVencimiento,
        Subtotal,
        Impuesto,
        Total,
        Estado
    )
    VALUES
    (
        @NumeroFactura,
        @ClienteNombre,
        @ClienteEmail,
        @FechaEmision,
        @FechaVencimiento,
        @Subtotal,
        @Impuesto,
        @Total,
        @Estado
    );

    SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_ObtenerFacturaPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        NumeroFactura,
        ClienteNombre,
        ClienteEmail,
        FechaEmision,
        FechaVencimiento,
        Subtotal,
        Impuesto,
        Total,
        Estado,
        CreadoEn
    FROM dbo.Facturas
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.SP_BuscarFacturasPorCliente
    @ClienteNombre VARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        NumeroFactura,
        ClienteNombre,
        ClienteEmail,
        FechaEmision,
        FechaVencimiento,
        Subtotal,
        Impuesto,
        Total,
        Estado,
        CreadoEn
    FROM dbo.Facturas
    WHERE ClienteNombre LIKE '%' + @ClienteNombre + '%'
    ORDER BY FechaEmision DESC;
END
GO
