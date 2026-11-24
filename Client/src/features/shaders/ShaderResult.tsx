
import { useEffect, useState } from "react";
import { shaderApi } from "../../api/shaderApi";
import { ShaderPreview } from "./ShaderPreview";
import type { GetShaderGenerationResponse } from "../../types/shader";

interface ShaderResultProps {
    generationId: string;
}

export function ShaderResult({ generationId }: ShaderResultProps) {
    const [generation, setGeneration] =
        useState<GetShaderGenerationResponse | null>(null);

    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const controller = new AbortController();

        let timeoutId: ReturnType<typeof setTimeout> | undefined;
        let stopped = false;

        setGeneration(null);
        setError(null);

        async function loadGeneration() {
            try {
                const result = await shaderApi.getGeneration(
                    generationId,
                    controller.signal
                );

                if (stopped) return;

                setGeneration(result);

                if (
                    result.status === "Pending" ||
                    result.status === "Processing"
                ) {
                    timeoutId = setTimeout(loadGeneration, 2000);
                }
            } catch (error) {
                if (controller.signal.aborted || stopped) return;

                setError(
                    error instanceof Error
                        ? error.message
                        : "Не удалось получить результат"
                );
            }
        }

        void loadGeneration();

        return () => {
            stopped = true;
            controller.abort();

            if (timeoutId !== undefined) {
                clearTimeout(timeoutId);
            }
        };
    }, [generationId]);

    if (error) {
        return (
            <section className="shader-result">
                <p className="error-message">{error}</p>
            </section>
        );
    }

    if (!generation) {
        return (
            <section className="shader-result">
                <p>Получаем информацию о генерации...</p>
            </section>
        );
    }

    return (
        <section className="shader-result">
            <h2>Результат генерации</h2>

            <p>
                Статус: <strong>{generation.status}</strong>
            </p>

            {generation.status === "Pending" && (
                <p>Задача ожидает обработки...</p>
            )}

            {generation.status === "Processing" && (
                <p>Генерируем шейдер...</p>
            )}

            {generation.status === "Failed" && (
                <p className="error-message">
                    Генерация завершилась ошибкой.
                </p>
            )}

            {generation.status === "Completed" && (
                <>
                    {generation.vertexShaderCode &&
                    generation.fragmentShaderCode ? (
                        <>
                            <h3>3D Preview</h3>

                            <ShaderPreview
                                geometryType={generation.geometryType}
                                vertexShader={generation.vertexShaderCode}
                                fragmentShader={generation.fragmentShaderCode}
                            />

                            <h3>Vertex Shader</h3>
                            <pre>
                                <code>{generation.vertexShaderCode}</code>
                            </pre>

                            <h3>Fragment Shader</h3>
                            <pre>
                                <code>{generation.fragmentShaderCode}</code>
                            </pre>
                        </>
                    ) : (
                        <p className="error-message">
                            Генерация завершена, но GLSL-код отсутствует.
                        </p>
                    )}
                </>
            )}
        </section>
    );
}
