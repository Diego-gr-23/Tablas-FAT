class FileSegment {
    public string Data { get; set; } // 20 caracteres maximo
    public string NextFilePath { get; set; } // Ruta del nuevo archivo de datos
    public bool EOF { get; set; } // Final del archivo
    

    public FileSegment(string data, string nextFilePath = null, bool eof = false)
    {
        Data = data;
        NextFilePath = nextFilePath;
        EOF = eof;
    }
}