export interface GenerateShaderRequest {
    userId: string;
    prompt: string;
}

export interface GenerateShaderResponse {
    generationId: string;
    status: ShaderGenerationStatus;
}

export interface GetShaderGenerationResponse {
    generationId: string;
    status: ShaderGenerationStatus;
    geometryType: GeometryType;
    vertexShaderCode: string | null;
    fragmentShaderCode: string | null;
}

export type ShaderGenerationStatus =
    | 'Pending'
    | 'Processing'
    | 'Completed'
    | 'Failed';

export type GeometryType =
    | "octahedron"
    | "icosahedron"
    | "sphere";