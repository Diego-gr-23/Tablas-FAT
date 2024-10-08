﻿using Newtonsoft.Json;
using System.Text;


class FatFiles {
    private static string FatTablePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FatTable.json");
    private static List<FATFiles> files = new List<FATFiles>();

    public static void LoadFATTable(){ // Se encargha de cargar la tabla FAT desde el archivo Json
        if(File.Exists(FatTablePath)){
            string json = File.ReadAllText(FatTablePath);
            files = JsonConvert.DeserializeObject<List<FATFiles>>(json) ?? new List<FATFiles>();
        }
    }

    public static void SaveFATTable() { // Guarda la tabla FAT en el archivo Json
        string json = JsonConvert.SerializeObject(files, Formatting.Indented);
        File.WriteAllText(FatTablePath, json);
    }

    public static void Creatfile() { // Se encarga de crear un archivo y repartir los datos en 20 segmentos
        Console.Write("Nombre del Archivo: ");
        string fileName = Console.ReadLine();

        Console.WriteLine("Ingrese el contenido del archivo (Precione ESC al finalizar)");
        string content = ReadUntilEscape();

        int size = content.Length;
        string firstSegmentPath = SaveSegments(content, fileName);

        FATFiles file = new FATFiles(fileName, firstSegmentPath, size);
        files.Add(file);

        SaveFATTable();
        Console.WriteLine("Archivo Creado exitosamente . \n");
        
    }

    private static string SaveSegments(string content, string fileName) { // Se encarga de guardan los datos y segmentos y se devuelve la ruta del primer segmento
        string currentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName + "_seg1.json");
        string previousPath  = null;

        for (int i = 0; i < content.Length; i += 20) { 
            string segmentData = content.Substring(i, Math.Min(20, content.Length - i));
            bool isEOF = (i + 20 >= content.Length);
            string nextPath = isEOF ? null: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName + "_seg" + (i / 20 + 2) + ".json");

            FileSegment segment = new FileSegment(segmentData, nextPath, isEOF);
            File.WriteAllText(currentPath, JsonConvert.SerializeObject(segment, Formatting.Indented));

            previousPath = currentPath;
            currentPath = nextPath;
        }

        return previousPath;
    }

    public static void ListFiles(bool includeDeleted = false) { // Se encarga de mostrar el listado de arhivos 
        int index = 1;
        foreach ( var file in files) {
            if (!includeDeleted && file.RecycleBinFlag) continue;

            Console.WriteLine($"{index} - {file.Name}, Tamaño: {file.Size} caracteres, Creado: {file.CreationDate}, Modificado: {file.ModificationDate}");
            index++;
        }
    }

    private static string ReadAllSegments(string firstSegmentPath) { // Se encarga de leer y concatenar todos los segmentos de un archivo
        string content = "";
        string currentPath = firstSegmentPath;

        while (currentPath != null){
            FileSegment segment = JsonConvert.DeserializeObject<FileSegment>(File.ReadAllText(currentPath));
            content += segment.Data;
            currentPath = segment.NextFilePath;
            
        }

        return content;
    }

    public static void OpenFile() { // Se encarga de abrir un archivo y mostrar el contenido completo concatenado
        Console.WriteLine("Archivos disponibles: ");
        ListFiles();

        Console.Write("\n seleccione el numero de arachivo a abrir: ");
        int choice = int.Parse(Console.ReadLine()) - 1;

        if (choice >= 0 && choice < files.Count){
            var file = files[choice];
            if (!file.RecycleBinFlag){
                Console.WriteLine($"Nombre: {file.Name}, Tamño: {file.Size} caracteres, Creado: {file.CreationDate}, Modificado: {file.ModificationDate}");
                Console.WriteLine($"contenido: \n {ReadAllSegments(file.DataPath)}");
            }
            else {
                Console.WriteLine("El archivo seleccionado esta en la papelera de reciclaje.");
            }
        }
    } 

    public static void ModifyFile() { // Se encarga de modificar un archivo y remplaza sus segmentos
        Console.WriteLine("Archivos disponibles:");
        ListFiles();

        Console.Write("\n Seleccione el numero de archivo a modificar: ");
        int choice = int.Parse(Console.ReadLine()) - 1;

        if(choice >= 0 && choice < files.Count) {
            var file = files[choice];
            Console.WriteLine($"Contenido actual:\n{ReadAllSegments(file.DataPath)}");
            Console.WriteLine("Ingresa los nuevos datos (hasta presionar ESC):");
            string newContent = ReadUntilEscape();

            Console.WriteLine("Desea guardar los cambios? (S / N)");
            if (Console.ReadLine(). ToUpper() == "S"){

                DeleteSegments(file.DataPath);

                file.DataPath = SaveSegments(newContent, file.Name);
                file.Size = newContent.Length;
                file.ModificationDate = DateTime.Now;

                SaveFATTable();
                Console.WriteLine("Arhivo modificado con exito.");
            }
        }
    }

    private static void DeleteSegments(string firstSegmentPath){// Se encarga de eliminar todos los segmentos de un archivo
        string currentPath = firstSegmentPath;

        while (currentPath != null) {
            FileSegment segment = JsonConvert.DeserializeObject<FileSegment>(File.ReadAllText(currentPath));
            File.Delete(currentPath);
            currentPath = segment.NextFilePath;
        }
    }

    private static string ReadUntilEscape(){ // Se encarga de leer los datos que ingresa el usuario hasta que presione la tecla ESC
        string content = "";
        ConsoleKeyInfo key;

        do {
            key = Console.ReadKey(true);
            if(key.Key != ConsoleKey.Escape){
                content += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        } while (key.Key != ConsoleKey.Escape);

        Console.WriteLine();
        return content;
    }

    public static void DeleteFile() { // Se encarga de elimina un archivo y actualiza la bandeja de la papelera
        Console.WriteLine("Archivos disponibles: ");
        ListFiles();

        Console.Write("\n Seleccione el numero de archivo a eliminar: ");
        int choice  = int.Parse(Console.ReadLine()) - 1;

        if (choice >= 0 && choice < files.Count) {
            var file = files[choice];
            
            Console.WriteLine($"Esta seguro de que desea eliminar el archivo '{file.Name}'? (S/N)");
            if (Console.ReadLine().ToUpper() == "S") { 
                file.RecycleBinFlag = true;
                file.DeletionDate = DateTime.Now;
                SaveFATTable();
                Console.WriteLine("Archivo eliminado exitosamente");
            }
        }
    }

    public static void RecoverFile(){ // Se encarga de recuperar un archivo de la papelera
        Console.WriteLine("Archivos en la papelera de reciclaje:");
        ListFiles(true);

        Console.Write("\n Seleccione el numero de archivo que desea recuperar: ");
        int choice = int.Parse(Console.ReadLine()) - 1;

        if (choice >= 0 && choice < files.Count){
            var file = files[choice];

            if (file.RecycleBinFlag) {
                Console.WriteLine($"¿Está seguro de que desea recuperar el archivo '{file.Name}'? (S/N)");
                if (Console.ReadLine().ToUpper() == "S"){
                    file.RecycleBinFlag = false;
                    file.DeletionDate = null;
                    SaveFATTable();
                    Console.WriteLine("Su archivo se recupero exitosamente");
                }
            } 
        }
    }
}