using System;
using System.Collections.Generic;

[Serializable]
public class JSONUsedMathQuestionData
{
    public List<int> UsedMathQuestionIds = new List<int>();
}

// This class tracks the IDs of math questions that have been used/answered.
// This the data model for saving/loading used question IDs in JSON format.
