# Estrategia de pruebas volumétricas para InvoiceService.Api

## 1) ¿Qué es una prueba volumétrica?

Una prueba volumétrica evalúa el comportamiento del sistema cuando opera con **grandes volúmenes de datos** (por ejemplo, millones de registros) y con consultas que deben mantenerse eficientes. El foco principal es validar:

- Capacidad de la base de datos para almacenar y consultar a escala.
- Degradación de tiempos de respuesta por crecimiento de datos.
- Efectos de índices, planes de ejecución y tamaño de respuesta.

### Diferencia vs. prueba de carga

- **Carga**: se centra en concurrencia y tasa de solicitudes (RPS) dentro de rangos esperados.
- **Volumétrica**: se centra en el tamaño del dataset y su impacto en rendimiento (concurrencia puede ser moderada).

### Diferencia vs. prueba de estrés

- **Estrés**: busca el punto de quiebre del sistema (más allá de lo esperado) y cómo se recupera.
- **Volumétrica**: busca que con mucho volumen de datos el sistema siga cumpliendo SLOs definidos.

## 2) Escenario propuesto (caso de uso realista)

### Contexto
Una entidad financiera consulta facturas asociadas a clientes para validación y atención. Con el tiempo se acumulan millones de facturas.

### APIs bajo prueba
- `POST /invoice` (registro de facturas)
- `GET /invoice/{id}` (consulta por id)
- `GET /invoice/search?client=...` (búsqueda por cliente)

### Volúmenes de datos
- **Dataset inicial**: 1,000,000 facturas.
- **Dataset objetivo**: 10,000,000 facturas.
- **Clientes**: 200,000 clientes distintos.
- Distribución:
  - 80% de clientes con 1–20 facturas.
  - 19% con 21–200.
  - 1% con 201–5,000.

### Transacciones
- **Inserción** (`POST /invoice`): 50–200 RPS durante ventanas de 30 min.
- **Consulta por id** (`GET /invoice/{id}`): 200–1,000 RPS.
- **Búsqueda por cliente** (`GET /invoice/search`): 100–500 RPS, con mezcla:
  - búsquedas que retornan 0 resultados
  - búsquedas que retornan pocos resultados
  - búsquedas que retornan muchos resultados

## 3) Métricas y KPIs

### API
- **P50/P95/P99 de latencia** por endpoint.
- **Throughput** (RPS) sostenido.
- **Tasa de errores** (4xx y 5xx) y su razón.
- **Tamaño de respuesta** promedio y máximo.

### Infra
- **CPU/RAM** de la API.
- **CPU/RAM/IOPS** de SQL Server.
- **Conexiones activas** a SQL Server.

### Base de datos
- **Duración de stored procedures** (promedio y percentiles).
- **Bloqueos y esperas** (wait stats).
- **Uso de índices** (seek vs scan).

### Herramientas sugeridas
- **k6** o **JMeter** para generar carga.
- **dotnet-counters** y **dotnet-trace** para métricas de la API.
- **SQL Server DMVs**: `sys.dm_exec_query_stats`, `sys.dm_db_index_usage_stats`, `sys.dm_os_wait_stats`.
- Logs estructurados de la API.

## 4) Estrategia de ejecución

### Fase A: Preparación de datos
- Generar e insertar dataset incremental (1M → 5M → 10M).
- Validar que el índice `IX_Facturas_ClienteNombre` exista.

### Fase B: Prueba volumétrica (dataset grande + carga moderada)
- Dataset: 10M.
- Ejecutar cargas por endpoint manteniendo RPS estable.
- Medir P95/P99 y errores.

### Fase C: Validación de regresión
- Comparar métricas entre 1M vs 10M.
- Si la latencia crece desproporcionadamente, revisar planes de ejecución.

### Criterios de éxito (ejemplo)
- `GET /invoice/{id}`: P95 < 150 ms.
- `GET /invoice/search`: P95 < 400 ms para clientes con <= 50 facturas.
- Error 5xx < 0.1%.
- Sin crecimiento sostenido de memoria (sin fuga) en la API.

## 5) Posibles cuellos de botella y soluciones

### Cuello de botella: búsqueda por cliente con `LIKE '%texto%'`
- **Síntoma**: scans y latencia creciente a medida que crece el dataset.
- **Soluciones**:
  - Buscar por prefijo (`LIKE 'texto%'`) cuando sea posible.
  - Normalizar columna para búsquedas (por ejemplo, `ClienteNombreNormalizado`) e indexarla.
  - Implementar paginación en el endpoint de búsqueda.

### Cuello de botella: pool de conexiones a SQL Server
- **Síntoma**: timeouts intermitentes y colas.
- **Soluciones**:
  - Verificar `SqlConnection` por request (ya se usa) y ajustar pool settings en connection string si aplica.
  - Evitar consultas innecesarias; revisar SPs.

### Cuello de botella: respuestas grandes en `/invoice/search`
- **Síntoma**: alto consumo de memoria y ancho de banda.
- **Soluciones**:
  - Paginación (page/size).
  - Limitar resultados máximos.
  - Campos proyectados (devolver solo lo necesario).

### Cuello de botella: índices insuficientes
- **Síntoma**: `GET /invoice/{id}` lento por IO (menos probable con PK).
- **Soluciones**:
  - Mantener PK clustered.
  - Revisar `INCLUDE` del índice para cubrir consultas.
