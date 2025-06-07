window.setupInfiniteScroll = (dotNetHelper) => {
    let isLoading = false;

    const handleScroll = () => {
        if (isLoading) return;

        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        const scrollHeight = document.documentElement.scrollHeight;
        const clientHeight = document.documentElement.clientHeight;

        // Load more when user is 200px from bottom
        if (scrollTop + clientHeight >= scrollHeight - 200) {
            isLoading = true;
            dotNetHelper.invokeMethodAsync('LoadMoreFromScroll').then(() => {
                setTimeout(() => isLoading = false, 1000);
            });
        }
    };

    window.addEventListener('scroll', handleScroll);
};

window.downloadFile = (fileName, content, contentType) => {
    const blob = new Blob([content], { type: contentType });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
};