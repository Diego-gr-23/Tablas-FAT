class FATFiles {
    public string Name { get; set;}
    public string DataPath { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set;}
    public bool RecycleBinFlag { get; set; }
    public DateTime? DeletionDate { get; set; }
    public int Size { get; set; }


    public FATFiles(string name, string dataPath, int size){
        Name = name;
        DataPath = dataPath;
        CreationDate = DateTime.Now;
        ModificationDate = DateTime.Now;
        Size = size;

    }

}
