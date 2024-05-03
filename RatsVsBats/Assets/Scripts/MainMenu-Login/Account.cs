using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Account : MonoBehaviour
{
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
    [SerializeField] private string URL;

    private void OnEnable()
    {
        editWarning.SetActive(false);

        string tableName = "Users";
        string[] columns = { "nickname", "email", "points", "branches", "missionsCompleted"};
        object[] values = { "developer@voliac-games.com" };

        /*if(Login.instance.isLogged) */GetData(tableName, columns, values);
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
        else gameObject.SetActive(false);
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
        // WHERE email = email.text
        string query = $"SELECT {columns[0]}, {columns[1]} FROM {tableName} WHERE {columns[1]} = {values[0].ToString()}";

        DataSet resultDataSet = DatabaseManager.instance.ExecuteQuery(query);

        if (resultDataSet != null && resultDataSet.Tables.Count > 0 && resultDataSet.Tables[0].Rows.Count > 0)
        {
            DataRow row = resultDataSet.Tables[0].Rows[0];

            nickname.text = row[columns[0]].ToString();
            email.text = row[columns[1]].ToString();
            points.text = row[columns[2]].ToString();
            historyBranches.text = $"{row[columns[3]].ToString()}/3";
            missionsCompleted.text = row[columns[4]].ToString();
        }
    }
}