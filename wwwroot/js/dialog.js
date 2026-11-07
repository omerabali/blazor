export function show(dialog, heading) {
    if (!dialog?.isConnected || dialog.open) {
        return;
    }

    if (document.activeElement === document.body) {
        heading?.focus({ preventScroll: true });
    }

    dialog.onclick = event => {
        if (event.target !== dialog) {
            return;
        }

        const rect = dialog.getBoundingClientRect();
        if (event.clientX < rect.left || event.clientX >= rect.right ||
            event.clientY < rect.top || event.clientY >= rect.bottom) {
            dialog.close();
        }
    };
    dialog.showModal();
}
