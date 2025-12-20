(function () {
    const originalFetch = window.fetch;
    window.fetch = async (url, options) => {

        const response = await originalFetch(url, options);

        // چک می‌کنیم آیا این درخواست لاگین بوده
        if (url.toLowerCase().includes("login") && response.ok) {
            const data = await response.clone().json();
            console.log(data);
            // فرض می‌کنیم token در فیلد data.token است
            const token = data.data.access_token;

            if (token) {
                const bearerToken = /*"Bearer " +*/ token;

                // ست‌کردن توکن داخل swagger
                window.ui.preauthorizeApiKey("Bearer", bearerToken);

                console.log("✔ Token auto-set in Swagger:", bearerToken);
            }
        }

        return response;
    };
})();
