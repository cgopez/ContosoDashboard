window.docUpload = {
  uploadWithProgress: function (fileInput, url, title, category, dotNetRef) {
    try {
      // support passing an element id (string) or the element itself
      if (typeof fileInput === 'string') {
        fileInput = document.getElementById(fileInput);
      }
      const files = fileInput && fileInput.files ? fileInput.files : null;
      if (!files || files.length === 0) {
        dotNetRef.invokeMethodAsync('UploadCompleted', 400, 'No files selected');
        return;
      }

      const formData = new FormData();
      for (let i = 0; i < files.length; i++) {
        formData.append('files', files[i], files[i].name);
      }
      if (title) formData.append('title', title);
      if (category) formData.append('category', category);

      const xhr = new XMLHttpRequest();
      xhr.open('POST', url, true);

      xhr.upload.onprogress = function (e) {
        if (e.lengthComputable) {
          const percent = Math.round((e.loaded / e.total) * 100);
          dotNetRef.invokeMethodAsync('ReportProgress', percent);
        }
      };

      xhr.onload = function () {
        dotNetRef.invokeMethodAsync('UploadCompleted', xhr.status, xhr.responseText || '');
      };

      xhr.onerror = function () {
        dotNetRef.invokeMethodAsync('UploadCompleted', xhr.status || 0, xhr.responseText || 'Network error');
      };

      xhr.send(formData);
    } catch (err) {
      dotNetRef.invokeMethodAsync('UploadCompleted', 0, err && err.toString ? err.toString() : 'Unknown error');
    }
  }
  ,getFileCount: function (inputId) {
    var fi = document.getElementById(inputId);
    return fi && fi.files ? fi.files.length : 0;
  }
  ,clearFileInput: function (inputId) {
    var fi = document.getElementById(inputId);
    if (fi) fi.value = '';
  }
};
