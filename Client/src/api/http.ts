export async function apiRequest<T>(
    url: string,
    options: RequestInit = {}
): Promise<T> {
    const headers = new Headers(options.headers);

    headers.set("Accept", "application/json");

    if (options.body) {
        headers.set("Content-Type", "application/json");
    }

    const response = await fetch(url, {
        ...options,
        headers
    });

    if (!response.ok) {
        const error = await response.text();

        throw new Error(
            error || `HTTP Error: ${response.status}`
        );
    }

    if (response.status === 204) {
        return undefined as T;
    }

    return response.json() as Promise<T>;
}