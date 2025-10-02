using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public List<int> answeredQuestionIds = new List<int>();
}

// This class currently only tracks answered question IDs.
// This class can be expanded in the future to include more game state data as needed.
