using Newtonsoft.Json;
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

     public static string ReadUntilEscape() { //Se encarga de leer cada tecla que presiona el usario hasta presionar ESC
        StringBuilder input = new StringBuilder();
        ConsoleKeyInfo key;

        Console.WriteLine("Escribe el contenido del archivo. Presiona ESC para finalizar:");

        do
        {
            key = Console.ReadKey(intercept: true); 

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
            else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                Console.Write("\b \b");  
                input.Length--; 
            }
            else if (key.Key != ConsoleKey.Backspace)
            {
                Console.Write(key.KeyChar); 
                input.Append(key.KeyChar);  
            }

        } while (key.Key != ConsoleKey.Escape);

        Console.WriteLine(); 
        return input.ToString();
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

    // static void Main(string[] args)
    // {
    //     while(true){
    //         Console.WriteLine("1- Crear un archivo");
    //         Console.WriteLine("2- Listar un archivo");
    //         Console.WriteLine("3- Abrir un archivo");
    //         Console.WriteLine("4- Modificar un archivo");
    //         Console.WriteLine("5- Eliminar un archivo");
    //         Console.WriteLine("6- Recuperar un archivo");
    //         Console.WriteLine("7- Salir");

    //         var option = Console.ReadLine();
    //         Console.Clear();

    //         if (option == "1"){
    //             Console.WriteLine("Nombre del Archivo: ");
    //             string fileName = Console.ReadLine();

    //             Console.WriteLine("Ingrese el contenido del archivo (Cuando termine presiona ESC):");
    //             string content = ReadUntilEscape();

    //             FATFiles file = new FATFiles(fileName, content);
    //             file.Add(file)

    //         }

    //     }
    // }
}