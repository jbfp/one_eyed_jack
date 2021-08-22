const GET_HASH_PATH = '/hash.txt';

const GET_HASH_REQUEST = Object.freeze({
    headers: { Accept: 'text/plain' },
});

export async function getHashAsync() {
    try {
        const response = await window.fetch(GET_HASH_PATH, GET_HASH_REQUEST);

        if (response.ok) {
            const body = await response.text();
            const hash = body.trim();
            return hash;
        }
    } catch {
        // Ignore.
    }

    return 'master';
}
