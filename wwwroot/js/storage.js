export function getItem(key) {
    return localStorage.getItem(key);
}

export function updateItem(key, update) {
    if (!navigator.locks) {
        throw new Error("Randevu yazmak için HTTPS veya localhost ve Web Locks desteği gerekir.");
    }

    return navigator.locks.request(key, async () => {
        const next = await update.invokeMethodAsync("Apply", localStorage.getItem(key));
        if (next === null) {
            return false;
        }

        localStorage.setItem(key, next);
        return true;
    });
}
