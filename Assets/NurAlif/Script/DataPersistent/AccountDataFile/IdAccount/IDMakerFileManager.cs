using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IDMakerFileManager
{
    private string accountDataPath = "";
    private string accountFolderName = "";
    private bool accountUseEcncryption = false;
    private readonly string encryptionCodeWord = "point&click";

    public IDMakerFileManager(string accountDataPath, string accountFolderName, bool useEcnryption)
    {
        this.accountDataPath = accountDataPath;
        this.accountFolderName = accountFolderName;
        this.accountUseEcncryption = useEcnryption;
    }

    public IDGameData LoadAccount()
    {
        string fullPath = Path.Combine(accountDataPath, accountFolderName);
        IDGameData loadedData = null;
        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (accountUseEcncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                //deserialized data yang diambil
                loadedData = JsonUtility.FromJson<IDGameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("Error saat ingin load data dari path : " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void SaveAccount(IDGameData data)
    {
        string fullPath = Path.Combine(accountDataPath,accountFolderName);
        try
        {
            //membuat directory untuk penyimpanan folder kalau belum ada
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //convert data di unity ke json
            string dataToStore = JsonUtility.ToJson(data,true);

            if (accountUseEcncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error saat ingin save account data ke path : " + fullPath + "\n" + e);
        }
    }

    //simple enkripsi dgn algoritma XOR
    public string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }

        return modifiedData;
    }
}
