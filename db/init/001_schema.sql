IF DB_ID('InvoiceDb') IS NULL
BEGIN
    CREATE DATABASE InvoiceDb;
END
GO

USE InvoiceDb;
GO

IF OBJECT_ID('dbo.Facturas', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Facturas
    (
        Id INT IDENTITY(1,1) NOT NULL,
        NumeroFactura VARCHAR(50) NOT NULL,
        ClienteNombre VARCHAR(200) NOT NULL,
        ClienteEmail VARCHAR(320) NOT NULL,
        FechaEmision DATETIME NOT NULL,
        FechaVencimiento DATETIME NOT NULL,
        Subtotal DECIMAL(18,2) NOT NULL,
        Impuesto DECIMAL(18,2) NOT NULL,
        Total DECIMAL(18,2) NOT NULL,
        Estado VARCHAR(20) NOT NULL,
        CreadoEn DATETIME NOT NULL CONSTRAINT DF_Facturas_CreadoEn DEFAULT (GETDATE()),
        CONSTRAINT PK_Facturas PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Facturas_NumeroFactura UNIQUE (NumeroFactura),
        CONSTRAINT CK_Facturas_Estado CHECK (Estado IN ('Pendiente','Pagada','Anulada')),
        CONSTRAINT CK_Facturas_Total CHECK (Total > 0),
        CONSTRAINT CK_Facturas_Fechas CHECK (FechaVencimiento > FechaEmision)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Facturas_ClienteNombre' AND object_id = OBJECT_ID('dbo.Facturas'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Facturas_ClienteNombre
    ON dbo.Facturas (ClienteNombre)
    INCLUDE (NumeroFactura, FechaEmision, Total, Estado);
END
GO
