// Download functionality for SVG files
window.downloadFile = (fileName, content, contentType) => {
    // Create a blob with the file content
    const blob = new Blob([content], { type: contentType });
    
    // Create a temporary URL for the blob
    const url = window.URL.createObjectURL(blob);
    
    // Create a temporary anchor element and trigger the download
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName;
    anchor.style.display = 'none';
    
    // Add to DOM, click, and remove
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);
    
    // Clean up the URL object
    window.URL.revokeObjectURL(url);
};

// Alternative method using data URL (fallback)
window.downloadFileDataUrl = (fileName, content, contentType) => {
    const dataUrl = `data:${contentType};charset=utf-8,${encodeURIComponent(content)}`;
    const anchor = document.createElement('a');
    anchor.href = dataUrl;
    anchor.download = fileName;
    anchor.style.display = 'none';
    
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);
};
