import { trigger, useValue, bindValue } from "cs2/api";
import { useState, useEffect, useRef, useCallback } from "react";

const selectedLineIndex$ = bindValue<number>('nightVehicles', 'selectedLineIndex', -1);
const nightVehicleCount$ = bindValue<number>('nightVehicles', 'nightVehicleCount', 1);
const schedule$ = bindValue<number>('nightVehicles', 'schedule', 2);
const maxVehicles$ = bindValue<number>('nightVehicles', 'maxVehicles', 1);

const DAY_AND_NIGHT = 2;

export const NightVehiclesSection = (VehicleCount: any) => (props: any) => {
    const selectedLine = useValue(selectedLineIndex$);
    const currentNightCount = useValue(nightVehicleCount$);
    const schedule = useValue(schedule$);
    const maxVehiclesBinding = useValue(maxVehicles$);
    const maxVehicles = (props.vehicleCountMax && props.vehicleCountMax > 1)
        ? props.vehicleCountMax
        : maxVehiclesBinding;
    const [nightCount, setNightCount] = useState<number>(1);
    const trackRef = useRef<HTMLDivElement>(null);
    const isDragging = useRef(false);

    useEffect(() => {
        setNightCount(Math.max(1, Math.min(currentNightCount, maxVehicles)));
    }, [selectedLine, currentNightCount, maxVehicles]);

    const getValueFromEvent = useCallback((clientX: number) => {
        if (!trackRef.current) return nightCount;
        const rect = trackRef.current.getBoundingClientRect();
        const pct = Math.max(0, Math.min(1, (clientX - rect.left) / rect.width));
        return Math.max(1, Math.min(maxVehicles, Math.round(1 + pct * (maxVehicles - 1))));
    }, [maxVehicles, nightCount]);

    const handlePointerDown = useCallback((e: any) => {
        isDragging.current = true;
        const value = getValueFromEvent(e.clientX);
        setNightCount(value);
        trigger('nightVehicles', 'setNightVehicles', selectedLine, value);
    }, [getValueFromEvent, selectedLine]);

    const handlePointerMove = useCallback((e: any) => {
        if (!isDragging.current) return;
        const value = getValueFromEvent(e.clientX);
        setNightCount(value);
    }, [getValueFromEvent]);

    const handlePointerUp = useCallback((e: any) => {
        if (!isDragging.current) return;
        isDragging.current = false;
        const value = getValueFromEvent(e.clientX);
        setNightCount(value);
        trigger('nightVehicles', 'setNightVehicles', selectedLine, value);
    }, [getValueFromEvent, selectedLine]);

    const showNightSlider = selectedLine >= 0 && schedule === DAY_AND_NIGHT;
    const pct = maxVehicles > 1 ? ((nightCount - 1) / (maxVehicles - 1)) * 100 : 0;

    return (
        <>
            <VehicleCount {...props} />
            {showNightSlider && (
                <div style={{ padding: '4rem 12rem 8rem 12rem', borderTop: '1px solid rgba(255,255,255,0.08)' }}>
                    <div style={{
                        display: 'flex',
                        justifyContent: 'space-between',
                        alignItems: 'center',
                        marginBottom: '8rem',
                        fontSize: '13rem',
                        color: 'rgba(255,255,255,0.7)',
                        textTransform: 'uppercase',
                        letterSpacing: '0.5px'
                    }}>
                        <span>Véhicules la nuit</span>
                        <span style={{ color: 'white' }}>{nightCount} / {maxVehicles}</span>
                    </div>
                    <div
                        ref={trackRef}
                        style={{ position: 'relative', height: '18rem', cursor: 'pointer' }}
                        onClick={(e: any) => {
                            const value = getValueFromEvent(e.clientX);
                            setNightCount(value);
                            trigger('nightVehicles', 'setNightVehicles', selectedLine, value);
                        }}
                    >
                        <div style={{
                            position: 'absolute',
                            top: '50%',
                            left: 0,
                            right: 0,
                            transform: 'translateY(-50%)',
                            height: '3rem',
                            background: 'rgba(255,255,255,0.15)',
                            borderRadius: '2rem',
                            overflow: 'hidden'
                        }}>
                            <div style={{ width: `${pct}%`, height: '100%', background: 'rgba(100,200,255,0.9)' }} />
                        </div>
                        <div
                            style={{
                                position: 'absolute',
                                top: '50%',
                                left: `${pct}%`,
                                transform: 'translate(-50%, -50%)',
                                width: '16rem',
                                height: '16rem',
                                background: 'white',
                                borderRadius: '50%',
                                boxShadow: '0 1px 4px rgba(0,0,0,0.5)',
                                cursor: 'grab'
                            }}
                            onMouseDown={(e: any) => {
                                e.stopPropagation();
                                const onMove = (moveEvent: MouseEvent) => {
                                    const value = getValueFromEvent(moveEvent.clientX);
                                    setNightCount(value);
                                };
                                const onUp = (upEvent: MouseEvent) => {
                                    const value = getValueFromEvent(upEvent.clientX);
                                    setNightCount(value);
                                    trigger('nightVehicles', 'setNightVehicles', selectedLine, value);
                                    document.removeEventListener('mousemove', onMove);
                                    document.removeEventListener('mouseup', onUp);
                                };
                                document.addEventListener('mousemove', onMove);
                                document.addEventListener('mouseup', onUp);
                            }}
                        />
                    </div>
                </div>
            )}
        </>
    );
};
