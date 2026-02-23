# Optimización de prompt para resumen

## Prompt original

```
Resume el siguiente texto: [En caso de revocación de la póliza o modificaciones de cualquiera de las condiciones
... (texto) ...
]. Devuelve solo un resumen corto y preciso.
```

## Prompt optimizado (versión mejorada)

```
Tarea: Resume el texto legal a continuación.

Objetivo del resumen:
- Explicar únicamente obligaciones, avisos y condiciones clave.
- Mantener términos y plazos exactos (días, montos, vigencia).
- No inventar información ni agregar recomendaciones.

Instrucciones de salida:
- Escribe 5 a 7 viñetas.
- Cada viñeta debe ser una frase corta.
- Incluye obligatoriamente:
  1) Aviso previo de 30 días por revocación o modificaciones.
  2) Plazo de 30 días calendario para notificar el siniestro a Seguros Sura.
  3) Plazo de 10 días hábiles para avisar a BANCO.
  4) Cobertura y aviso por terminación automática por mora.
  5) Monto asegurado y coberturas principales.
- No incluyas encabezados, introducciones ni conclusiones.
- No repitas el texto original.

Texto:
"""
[PEGAR TEXTO AQUÍ]
"""
```

## ¿Qué mejoras hice y por qué?

- **Claridad de objetivo**: se define qué debe contener el resumen (obligaciones/avisos/condiciones clave) para evitar que el modelo se vaya a detalles secundarios.
- **Control de formato**: se fija un rango de 5–7 viñetas y frases cortas para lograr concisión y consistencia.
- **Invariantes obligatorias**: se obliga a incluir plazos críticos y puntos contractuales (30 días, 10 días hábiles, mora, monto asegurado) para aumentar precisión.
- **Restricción anti-deriva**: se prohíbe inventar, recomendar o agregar interpretación, lo que reduce contenido irrelevante.

## Cómo esta versión evita respuestas irrelevantes

- **Límites explícitos**: al indicar “explicar únicamente obligaciones, avisos y condiciones clave”, se descartan elementos que no aporten a la intención del resumen.
- **Campos obligatorios**: fuerza a cubrir los puntos más importantes aunque el texto sea largo.
- **Prohibición de recomendaciones**: evita consejos o explicaciones fuera del documento.
- **Formato acotado**: un número fijo de viñetas evita respuestas extensas o divagaciones.

## Ejemplo comparativo

### Prompt original (riesgo)

- Suele producir un párrafo largo.
- Puede omitir plazos críticos.
- Puede incluir explicaciones generales sobre seguros.

### Prompt optimizado (resultado esperado)

- 5–7 viñetas, concisas.
- Plazos y montos exactos incluidos.
- Solo condiciones relevantes sin interpretación.

## Fuentes

Las mejoras aplicadas al prompt se basan en prácticas generales de ingeniería de prompts y control de salida (definir el rol/tarea, establecer restricciones explícitas, forzar formato y criterios de inclusión/exclusión para minimizar “deriva” y contenido irrelevante).

Referencias consultables:

- OpenAI. **Prompt engineering** (guía / buenas prácticas).
  - https://platform.openai.com/docs/guides/prompt-engineering
- OpenAI. **Best practices for prompt engineering** (colección de recomendaciones prácticas).
  - https://help.openai.com/en/articles/6654000-best-practices-for-prompt-engineering-with-openai-api
- Anthropic. **Prompt engineering** (técnicas para instrucciones claras y control de formato).
  - https://docs.anthropic.com/en/docs/build-with-claude/prompt-engineering
- Microsoft. **Prompt engineering** (principios para mejorar precisión y reducir respuestas irrelevantes).
  - https://learn.microsoft.com/azure/ai-services/openai/concepts/prompt-engineering

Nota: El contenido del “prompt optimizado” no proviene de fuentes externas; es una composición propia a partir de las prácticas anteriores aplicada al texto del caso.
