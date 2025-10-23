#!/bin/bash
# ============================================
# SCRIPT DE MIGRACIONES - APPOINTMENTS
# ============================================
# Proyecto: Sistema de Gestión de Turnos Médicos
# Microservicio: Appointments
# Propósito: Ejecutar scripts SQL en orden
# ============================================

set -e

echo ""
echo "═══════════════════════════════════════════════════════"
echo "🚀 INICIANDO MIGRACIONES - MICROSERVICIO APPOINTMENTS"
echo "═══════════════════════════════════════════════════════"
echo ""

# Verificar variables de entorno
if [ -z "$POSTGRES_HOST" ]; then
    echo "❌ ERROR: POSTGRES_HOST no está definido"
    exit 1
fi

if [ -z "$POSTGRES_USER" ]; then
    echo "❌ ERROR: POSTGRES_USER no está definido"
    exit 1
fi

if [ -z "$POSTGRES_PASSWORD" ]; then
    echo "❌ ERROR: POSTGRES_PASSWORD no está definido"
    exit 1
fi

if [ -z "$POSTGRES_DB" ]; then
    echo "❌ ERROR: POSTGRES_DB no está definido"
    exit 1
fi

echo "📋 Configuración:"
echo "  - Host: $POSTGRES_HOST"
echo "  - Usuario: $POSTGRES_USER"
echo "  - Base de datos: $POSTGRES_DB"
echo ""

# Esperar a que PostgreSQL esté listo
echo "⏳ Esperando a que PostgreSQL esté listo..."
until PGPASSWORD=$POSTGRES_PASSWORD psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d postgres -c '\q' 2>/dev/null; do
    echo "   PostgreSQL no está listo aún - reintentando en 2 segundos..."
    sleep 2
done
echo "✅ PostgreSQL está listo!"
echo ""

# Crear la base de datos si no existe
echo "📦 Creando base de datos '$POSTGRES_DB' si no existe..."
PGPASSWORD=$POSTGRES_PASSWORD psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d postgres <<-EOSQL
    SELECT 'CREATE DATABASE $POSTGRES_DB'
    WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '$POSTGRES_DB')\gexec
EOSQL
echo "✅ Base de datos verificada"
echo ""

# Función para ejecutar un script SQL
execute_sql() {
    local script=$1
    local description=$2
    
    echo "────────────────────────────────────────────────────────"
    echo "📄 Ejecutando: $description"
    echo "   Archivo: $script"
    echo "────────────────────────────────────────────────────────"
    
    if [ ! -f "$script" ]; then
        echo "❌ ERROR: Archivo no encontrado: $script"
        exit 1
    fi
    
    PGPASSWORD=$POSTGRES_PASSWORD psql \
        -h "$POSTGRES_HOST" \
        -U "$POSTGRES_USER" \
        -d "$POSTGRES_DB" \
        -f "$script" \
        --set ON_ERROR_STOP=on \
        -v ON_ERROR_STOP=1
    
    if [ $? -eq 0 ]; then
        echo "✅ Script ejecutado exitosamente"
    else
        echo "❌ ERROR al ejecutar el script"
        exit 1
    fi
    echo ""
}

# Ejecutar scripts en orden
echo "🔄 Ejecutando scripts de migración..."
echo ""

execute_sql "001_CreateTables.sql" "Creación de tablas"
execute_sql "002_CreateConstraints.sql" "Constraints y claves"
execute_sql "003_CreateIndexes.sql" "Índices de optimización"
execute_sql "004_SeedData.sql" "Datos iniciales"

# Verificación final
echo "═══════════════════════════════════════════════════════"
echo "🔍 VERIFICACIÓN FINAL"
echo "═══════════════════════════════════════════════════════"
echo ""

PGPASSWORD=$POSTGRES_PASSWORD psql \
    -h "$POSTGRES_HOST" \
    -U "$POSTGRES_USER" \
    -d "$POSTGRES_DB" \
    <<-EOSQL
    
    -- Contar tablas
    SELECT 
        '📊 Tablas creadas: ' || COUNT(*) AS info
    FROM information_schema.tables 
    WHERE table_schema = 'public' 
    AND table_type = 'BASE TABLE';
    
    -- Contar índices
    SELECT 
        '🔍 Índices creados: ' || COUNT(*) AS info
    FROM pg_indexes 
    WHERE schemaname = 'public';
    
    -- Contar constraints
    SELECT 
        '🔒 Constraints: ' || COUNT(*) AS info
    FROM information_schema.table_constraints 
    WHERE table_schema = 'public';
    
    -- Datos iniciales
    SELECT '📌 Estados: ' || COUNT(*) AS info FROM appointment_status;
    SELECT '📝 Motivos: ' || COUNT(*) AS info FROM appointment_reason;
    SELECT '🎉 Feriados 2025: ' || COUNT(*) AS info FROM national_holidays;
    
EOSQL

echo ""
echo "═══════════════════════════════════════════════════════"
echo "✅ MIGRACIONES COMPLETADAS EXITOSAMENTE"
echo "═══════════════════════════════════════════════════════"
echo ""
echo "🎯 Base de datos '$POSTGRES_DB' está lista para usar"
echo ""
echo "📋 Próximos pasos:"
echo "  1. Levantar la API del microservicio"
echo "  2. Configurar los horarios de los profesionales"
echo "  3. Generar slots automáticos"
echo "  4. Comenzar a programar turnos"
echo ""
echo "═══════════════════════════════════════════════════════"
