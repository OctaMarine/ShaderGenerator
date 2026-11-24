import { useState, type FormEvent } from "react";
import { shaderApi } from "../../api/shaderApi";
import type { GenerateShaderRequest } from "../../types/shader";

interface ShaderGeneratorFormProps {
    userId: string;
    onGenerationStarted: (generationId: string) => void;
}

export function ShaderGeneratorForm({
                                        userId,
                                        onGenerationStarted
                                    }: ShaderGeneratorFormProps) {
    const [prompt, setPrompt] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();

        setLoading(true);
        setError(null);

        try {
            const request: GenerateShaderRequest = {
                userId,
                prompt: prompt.trim()
            };

            const response = await shaderApi.generate(request);

            onGenerationStarted(response.generationId);
            setPrompt("");
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : "Не удалось запустить генерацию"
            );
        } finally {
            setLoading(false);
        }
    }

    return (
        <form onSubmit={handleSubmit} className="shader-form">
            <h2>Генерация шейдера</h2>

            <label htmlFor="prompt">
                Опиши, какой шейдер нужно создать
            </label>

            <textarea
                id="prompt"
                value={prompt}
                onChange={(event) => setPrompt(event.target.value)}
                placeholder="Create a glowing blue crystal..."
                rows={5}
                required
            />

            {error && (
                <p className="error-message">{error}</p>
            )}

            <button
                type="submit"
                disabled={loading || !prompt.trim()}
            >
                {loading ? "Отправляем..." : "Сгенерировать"}
            </button>
        </form>
    );
}