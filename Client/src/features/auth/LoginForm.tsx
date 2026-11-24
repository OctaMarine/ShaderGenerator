
import { useState, type FormEvent } from "react";
import { userApi } from "../../api/userApi";
import type { LoginResponse } from "../../types/user";

interface LoginFormProps {
    onLogin: (user: LoginResponse) => void;
}

export function LoginForm({ onLogin }: LoginFormProps) {
    const [login, setLogin] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();

        setError(null);
        setLoading(true);

        try {
            const user = await userApi.login(login.trim());
            onLogin(user);
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : "Не удалось выполнить вход"
            );
        } finally {
            setLoading(false);
        }
    }

    return (
        <form onSubmit={handleSubmit} className="login-form">
            <h2>Вход в Shader Generator</h2>

            <label htmlFor="login">Ваш логин</label>

            <input
                id="login"
                type="text"
                value={login}
                onChange={(event) => setLogin(event.target.value)}
                placeholder="Введите логин"
                required
                maxLength={100}
            />

            {error && (
                <p className="error-message">{error}</p>
            )}

            <button type="submit" disabled={loading || !login.trim()}>
                {loading ? "Входим..." : "Войти"}
            </button>
        </form>
    );
}
