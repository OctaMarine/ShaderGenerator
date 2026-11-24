import type { LoginRequest, LoginResponse } from "../types/user";
import { apiRequest } from "./http";

const BASE_URL = import.meta.env.VITE_USER_API_URL;

export const userApi = {
    login(login: string): Promise<LoginResponse> {
        const request: LoginRequest = { login };

        return apiRequest<LoginResponse>(
            `${BASE_URL}/api/users/login`,
            {
                method: "POST",
                body: JSON.stringify(request)
            }
        );
    }
};