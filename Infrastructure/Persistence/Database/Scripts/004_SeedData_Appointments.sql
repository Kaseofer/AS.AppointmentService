-- ============================================
-- DATOS INICIALES (SEED DATA) - MICROSERVICIO APPOINTMENTS
-- ============================================
-- Proyecto: Sistema de Gestión de Turnos Médicos
-- Microservicio: Appointments
-- Descripción: Estados, motivos y feriados del sistema
-- ============================================

-- ============================================
-- ESTADOS DE TURNOS
-- ============================================

INSERT INTO appointment_status (name, description, color, is_active) VALUES
    ('Programado', 'Turno programado y confirmado', '#007bff', true),
    ('Completado', 'Turno realizado exitosamente', '#28a745', true),
    ('Cancelado', 'Turno cancelado por paciente o profesional', '#dc3545', true),
    ('No asistió', 'Paciente no se presentó al turno', '#ffc107', true),
    ('En curso', 'Atención médica en progreso', '#17a2b8', true),
    ('Reagendado', 'Turno movido a otra fecha/hora', '#6c757d', true)
ON CONFLICT (name) DO UPDATE SET
    description = EXCLUDED.description,
    color = EXCLUDED.color,
    is_active = EXCLUDED.is_active;

-- ============================================
-- MOTIVOS DE CONSULTA
-- ============================================

INSERT INTO appointment_reason (name, description, is_active) VALUES
    ('Consulta general', 'Consulta médica de rutina', true),
    ('Control', 'Control de seguimiento', true),
    ('Emergencia', 'Atención de emergencia', true),
    ('Resultado de estudios', 'Revisión de resultados médicos', true),
    ('Certificado médico', 'Emisión de certificado', true),
    ('Vacunación', 'Aplicación de vacunas', true),
    ('Receta', 'Emisión o renovación de receta', true),
    ('Curaciones', 'Curaciones y procedimientos menores', true),
    ('Primera consulta', 'Primera vez con el profesional', true),
    ('Interconsulta', 'Derivación de otro profesional', true)
ON CONFLICT (name) DO UPDATE SET
    description = EXCLUDED.description,
    is_active = EXCLUDED.is_active;

-- ============================================
-- FERIADOS NACIONALES ARGENTINA 2025
-- ============================================

INSERT INTO national_holidays (holiday_date, name, description, is_movable) VALUES
    ('2025-01-01', 'Año Nuevo', 'Año Nuevo', false),
    ('2025-02-24', 'Carnaval', 'Lunes de Carnaval', true),
    ('2025-02-25', 'Carnaval', 'Martes de Carnaval', true),
    ('2025-03-24', 'Día Nacional de la Memoria por la Verdad y la Justicia', 'Memoria, Verdad y Justicia', false),
    ('2025-04-02', 'Día del Veterano y de los Caídos en la Guerra de Malvinas', 'Día de los Veteranos de Malvinas', false),
    ('2025-04-18', 'Viernes Santo', 'Viernes Santo Pascuas', true),
    ('2025-05-01', 'Día del Trabajador', 'Día Internacional del Trabajador', false),
    ('2025-05-25', 'Día de la Revolución de Mayo', 'Revolución de Mayo de 1810', false),
    ('2025-06-16', 'Paso a la Inmortalidad del General Güemes', 'Día de Güemes', true),
    ('2025-06-20', 'Paso a la Inmortalidad del General Belgrano', 'Día de la Bandera', false),
    ('2025-07-09', 'Día de la Independencia', 'Declaración de la Independencia', false),
    ('2025-08-17', 'Paso a la Inmortalidad del General San Martín', 'Día de San Martín', true),
    ('2025-10-12', 'Día del Respeto a la Diversidad Cultural', 'Día de la Raza', true),
    ('2025-11-24', 'Día de la Soberanía Nacional', 'Día de la Soberanía Nacional', true),
    ('2025-12-08', 'Inmaculada Concepción de María', 'Inmaculada Concepción', false),
    ('2025-12-25', 'Navidad', 'Navidad', false),
    ('2025-03-21', 'Feriado con fines turísticos', 'Fin de semana largo marzo', true)
ON CONFLICT (holiday_date) DO UPDATE SET
    name = EXCLUDED.name,
    description = EXCLUDED.description,
    is_movable = EXCLUDED.is_movable;

-- ============================================
-- VERIFICACIÓN
-- ============================================

DO $$
DECLARE
    v_status_count INTEGER;
    v_reason_count INTEGER;
    v_holiday_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO v_status_count FROM appointment_status;
    SELECT COUNT(*) INTO v_reason_count FROM appointment_reason;
    SELECT COUNT(*) INTO v_holiday_count FROM national_holidays;
    
    RAISE NOTICE '';
    RAISE NOTICE '═══════════════════════════════════════════════════════';
    RAISE NOTICE '✓ DATOS INICIALES INSERTADOS';
    RAISE NOTICE '═══════════════════════════════════════════════════════';
    RAISE NOTICE '';
    RAISE NOTICE '📊 Resumen:';
    RAISE NOTICE '  - Estados de turnos: %', v_status_count;
    RAISE NOTICE '  - Motivos de consulta: %', v_reason_count;
    RAISE NOTICE '  - Feriados nacionales 2025: %', v_holiday_count;
    RAISE NOTICE '';
    RAISE NOTICE '📌 Estados disponibles:';
    RAISE NOTICE '  1. Programado (azul)';
    RAISE NOTICE '  2. Completado (verde)';
    RAISE NOTICE '  3. Cancelado (rojo)';
    RAISE NOTICE '  4. No asistió (amarillo)';
    RAISE NOTICE '  5. En curso (celeste)';
    RAISE NOTICE '  6. Reagendado (gris)';
    RAISE NOTICE '';
    RAISE NOTICE '📝 Motivos más comunes:';
    RAISE NOTICE '  - Consulta general';
    RAISE NOTICE '  - Control';
    RAISE NOTICE '  - Primera consulta';
    RAISE NOTICE '';
    RAISE NOTICE '🎉 Feriados 2025:';
    RAISE NOTICE '  - % feriados nacionales registrados', v_holiday_count;
    RAISE NOTICE '  - Incluye Año Nuevo, Semana Santa, Independencia, Navidad';
    RAISE NOTICE '';
    RAISE NOTICE '⚠️  IMPORTANTE:';
    RAISE NOTICE '  - Los estados NO deben modificarse (sistema depende de ellos)';
    RAISE NOTICE '  - Los feriados se pueden actualizar anualmente';
    RAISE NOTICE '  - Los motivos se pueden ampliar según necesidad';
    RAISE NOTICE '';
    RAISE NOTICE '═══════════════════════════════════════════════════════';
    RAISE NOTICE '';
END $$;
