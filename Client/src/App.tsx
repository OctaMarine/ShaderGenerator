import { useState } from "react";

import { LoginForm } from "./features/auth/LoginForm";
import { ShaderGeneratorForm } from "./features/shaders/ShaderGeneratorForm";
import { ShaderResult } from "./features/shaders/ShaderResult";

import type { LoginResponse } from "./types/user";

import "./App.css";

function App() {
    const [user, setUser] = useState<LoginResponse | null>(null);
    const [generationId, setGenerationId] = useState<string | null>(null);

    if (!user) {
        return (
            <main className="app-container">
                <LoginForm onLogin={setUser} />
            </main>
        );
    }

    return (
        <main className="app-container">
            <section className="user-panel">
                <h2>Добро пожаловать, {user.login}!</h2>

                <button
                    type="button"
                    onClick={() => {
                        setUser(null);
                        setGenerationId(null);
                    }}
                >
                    Выйти
                </button>
            </section>

            <ShaderGeneratorForm
                userId={user.id}
                onGenerationStarted={setGenerationId}
            />

            {generationId && (
                <ShaderResult
                    key={generationId}
                    generationId={generationId}
                />
            )}
        </main>
    );
}

export default App;