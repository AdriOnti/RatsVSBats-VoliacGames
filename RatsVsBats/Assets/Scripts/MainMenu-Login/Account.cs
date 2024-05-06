using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Account : MonoBehaviour
{
    public static Account Instance;
    [Header("Icon")]
    [SerializeField] private RawImage profileIcon;

    [Header("TMP")]
    [SerializeField] private TextMeshProUGUI nickname;
    [SerializeField] private TextMeshProUGUI email;
    [SerializeField] private TextMeshProUGUI points;
    [SerializeField] private TextMeshProUGUI historyBranches;
    [SerializeField] private TextMeshProUGUI missionsCompleted;

    [Header("Edit")]
    [SerializeField] private GameObject editWarning;
    [SerializeField] private GameObject account;
    [SerializeField] private string URL;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        editWarning.SetActive(false);
    }

    public void JustLogged()
    {
        string tableName = "Profiles";
        string[] columns = { "idProfiles", "nickName", "completedMissions", "completedBranches", "points" };
        object[] values = { Login.instance.idUser, Login.instance.email.text };

        if (Login.instance.isLogged) GetData(tableName, columns, values);
    }

    /// <summary>
    /// Open the warning
    /// </summary>
    public void EditProfile() { editWarning.SetActive(true); }

    /// <summary>
    /// Close the actual game object
    /// </summary>
    /// <param name="isWarning">If is true, only closes the warning</param>
    public void CloseObject(bool isWarning)
    {
        if(isWarning) editWarning.SetActive(false);
        else account.SetActive(false);
        CursorManager.Instance.ResetCursor();
    }

    /// <summary>
    /// Go to the Website for change the data
    /// </summary>
    public void GoToWebsite() { Application.OpenURL(URL); }

    /// <summary>
    /// Get all the information from the SQL query
    /// </summary>
    /// <param name="tableName">Name of the table of the database</param>
    /// <param name="columns">Columns of the table</param>
    /// <param name="values">The values to check in the where</param>
    public void GetData(string tableName, string[] columns, object[] values)
    {
        // WHERE idProfiles = idUsers
        string query = $"SELECT {columns[1]}, {columns[2]}, {columns[3]}, {columns[4]} FROM {tableName} WHERE {columns[0]} = {values[0]}";

        DataSet resultDataSet = DatabaseManager.instance.ExecuteQuery(query);

        if (resultDataSet != null && resultDataSet.Tables.Count > 0 && resultDataSet.Tables[0].Rows.Count > 0)
        {
            DataRow row = resultDataSet.Tables[0].Rows[0];

            nickname.text = row[columns[1]].ToString();
            email.text = values[1].ToString();
            missionsCompleted.text = row[columns[2]].ToString();
            historyBranches.text = $"{row[columns[3]]}/3";
            points.text = row[columns[4]].ToString();
        }
    }
}