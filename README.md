# Flujo de trabajo Git

## Estructura de ramas

| Rama | Propósito |
|------|-----------|
| `main` | Versiones jugables y estables. **Nadie toca esto directamente.** |
| `dev` | Rama de integración. Todo el trabajo de desarrollo se fusiona aquí. |
| `des` | Rama de integración para diseño. Assets, sprites, UI y animaciones. |
| `dev_nombre_feature` | Rama personal de cada desarrollador. |
| `des_nombre_feature` | Rama personal de cada diseñador. |

---

## Flujo del día a día

**Desarrolladores** parten siempre desde `dev`:
```bash
git checkout dev
git pull origin dev
git checkout -b dev_tunombre_feature
```

**Diseñadores** parten siempre desde `des`:
```bash
git checkout des
git pull origin des
git checkout -b des_tunombre_feature
```

Cuando el trabajo está listo, se abre un **Pull Request** hacia `dev` o `des` según corresponda. Otra persona lo revisa antes de mergear.

Cuando `dev` y `des` están estables y jugables, se hace merge a `main` como nuevo hito.

---

## Reglas

- **Nunca** commitear directamente a `main`, `dev` ni `des`.
- Hacer `pull` de tu rama base (`dev` o `des`) para evitar conflictos grandes.
- Las ramas personales son cortas: se crean, se mergean y se borran.
- Nombrar las ramas de forma descriptiva: `dev_nombre_feature`.

---

## Convención de commits

Formato: `tipo: descripción breve en minúsculas`

| Tipo | Cuándo usarlo |
|------|---------------|
| `feat:` | Nueva funcionalidad |
| `fix:` | Corrección de un bug |
| `art:` | Assets, sprites, audio, animaciones |
| `ui:` | Cambios visuales de interfaz |
| `refactor:` | Reorganización de código sin cambiar comportamiento |
| `docs:` | Cambios en documentación |
| `chore:` | Tareas de mantenimiento (gitignore, configuración…) |

**Ejemplos:**
```
feat: añadir salto doble al jugador
fix: corregir colisión con plataformas móviles
art: añadir spritesheet del enemigo básico
ui: ajustar posición del marcador de puntuación
```

**Reglas:**
- Descripción corta en minúsculas y en infinitivo
- Sin punto final

---
