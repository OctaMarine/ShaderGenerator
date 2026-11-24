import type {
    GenerateShaderRequest,
    GenerateShaderResponse,
    GetShaderGenerationResponse
} from "../types/shader";

import { apiRequest } from "./http";

const BASE_URL = import.meta.env.VITE_SHADER_API_URL;

export const shaderApi = {
    generate(request: GenerateShaderRequest): Promise<GenerateShaderResponse> {
        return apiRequest<GenerateShaderResponse>(
            `${BASE_URL}/api/shaders/generate`,
            {
                method: "POST",
                body: JSON.stringify(request)
            }
        );
    },

    getGeneration(
        generationId: string,
        signal?: AbortSignal
    ): Promise<GetShaderGenerationResponse> {
        return apiRequest<GetShaderGenerationResponse>(
            `${BASE_URL}/api/shaders/${encodeURIComponent(generationId)}`,
            { signal }
        );
    }
};