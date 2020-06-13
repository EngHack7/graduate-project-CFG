





































namespace DrawCFGraph.CParser.CGrammar
{
    using CGrammar;
    using global::CGrammar;
    using global::CParser;
    using System.Collections.Generic;

    public sealed class CParser
    {
        public static readonly IMatchable struct_or_union = new CTokenAlternative(
            new CToken(CTokenType.STRUCT),
            new CToken(CTokenType.UNION)
        );

        public static readonly IMatchable compound_statement = new CTokenEnclosed(new CToken("{"), new CToken("}"));

        public static readonly IMatchable struct_or_union_specifier = new CTokenAlternative(
            new CTokenSequence(struct_or_union, new CToken(CTokenType.IDENTIFIER), compound_statement),
            new CTokenSequence(struct_or_union, new CTokenEnclosed(new CToken("{"), new CToken("}"))),
            new CTokenSequence(struct_or_union, new CToken(CTokenType.IDENTIFIER))
        );

        public static readonly IMatchable enum_specifier = new CTokenAlternative(
            new CTokenSequence(new CToken(CTokenType.ENUM), compound_statement),
            new CTokenSequence(new CToken(CTokenType.ENUM), new CToken(CTokenType.IDENTIFIER), compound_statement),
            new CTokenSequence(new CToken(CTokenType.ENUM), new CToken(CTokenType.IDENTIFIER))
        );

        public static readonly IMatchable storage_class_specifier = new CTokenAlternative(
            new CToken(CTokenType.TYPEDEF),
            new CToken(CTokenType.EXTERN),
            new CToken(CTokenType.STATIC),
            new CToken(CTokenType.AUTO),
            new CToken(CTokenType.REGISTER)
        );

        public static readonly IMatchable type_specifier = new CTokenAlternative(
            new CToken(CTokenType.VOID),
            new CToken(CTokenType.CHAR),
            new CToken(CTokenType.SHORT),
            new CToken(CTokenType.INT),
            new CToken(CTokenType.LONG),
            new CToken(CTokenType.FLOAT),
            new CToken(CTokenType.DOUBLE),
            new CToken(CTokenType.SIGNED),
            new CToken(CTokenType.UNSIGNED),
            struct_or_union_specifier,
            enum_specifier,
            new CToken(CTokenType.TYPE_NAME)
        );

        public static readonly IMatchable type_qualifier = new CTokenAlternative(
            new CToken(CTokenType.CONST),
            new CToken(CTokenType.VOLATILE)
        );

        public static readonly IMatchable declaration_specifiers_repeat = new CTokenRepeat(
            new CTokenAlternative(
                storage_class_specifier,
                type_specifier,
                type_specifier,
                type_qualifier
            ),
            true
        );

        public static readonly IMatchable declaration_specifiers = new CTokenAlternative(
            storage_class_specifier,
            new CTokenSequence(storage_class_specifier, declaration_specifiers_repeat),
            type_specifier,
            new CTokenSequence(type_specifier, declaration_specifiers_repeat),
            type_qualifier,
            new CTokenSequence(type_qualifier, declaration_specifiers_repeat)
        );

        public static readonly IMatchable type_qualifier_list = new CTokenRepeat(type_qualifier, true);

        public static readonly IMatchable pointer_repeat = new CTokenRepeat(
            new CTokenAlternative(
                new CToken("*"),
                new CTokenSequence(new CToken("*"), type_qualifier_list)
            ),
            true
        );

        public static readonly IMatchable pointer = new CTokenAlternative(
            new CToken("*"),
            new CTokenSequence(new CToken("*"), type_qualifier_list),
            new CTokenSequence(new CToken("*"), pointer_repeat),
            new CTokenSequence(new CToken("*"), type_qualifier_list, pointer_repeat)
        );

        public static readonly IMatchable direct_declarator = new CTokenSequence(
            new CToken(CTokenType.IDENTIFIER),
            new CTokenRepeat(
                new CTokenAlternative(
                    new CTokenEnclosed(new CToken("("), new CToken(")")),
                    new CTokenEnclosed(new CToken("["), new CToken("]"))
                ),
                true, 1
            )
        );

        public static readonly IMatchable declarator = new CTokenAlternative(
            new CTokenSequence(pointer, direct_declarator),
            direct_declarator
        );

        public static readonly IMatchable function_declarator = new CTokenAlternative(
            new CTokenSequence(declaration_specifiers, declarator),
            declarator
        );

        public static readonly IMatchable jump_statement = new CTokenAlternative(
            new CTokenSequence(new CToken(CTokenType.GOTO), new CToken(CTokenType.IDENTIFIER), new CToken(";")),
            new CTokenSequence(new CToken(CTokenType.CONTINUE), new CToken(";")),
            new CTokenSequence(new CToken(CTokenType.BREAK), new CToken(";")),
            new CTokenSequence(new CToken(CTokenType.RETURN), new CToken(";")),
            new CTokenSequence(new CToken(CTokenType.RETURN), new CTokenRepeat(CToken.anyToken, false), new CToken(";"))
        );

        public static readonly IMatchable valid_expression_tokens = CToken.anyTokenBesides(
            CTokenType.TYPEDEF,
            CTokenType.ELLIPSIS,
            CTokenType.CASE,
            CTokenType.DEFAULT,
            CTokenType.IF,
            CTokenType.ELSE,
            CTokenType.SWITCH,
            CTokenType.WHILE,
            CTokenType.DO,
            CTokenType.FOR,
            CTokenType.GOTO,
            CTokenType.CONTINUE,
            CTokenType.BREAK,
            CTokenType.RETURN,
            CTokenType.LEFT_BRACE,
            CTokenType.RIGHT_BRACE,
            CTokenType.SEMICOLON
        );

        public static readonly IMatchable expression_repeat = new CTokenRepeat(valid_expression_tokens, false);

        public static readonly IMatchable labeled_statement = new CTokenAlternative(
            new CTokenSequence(new CToken(CTokenType.IDENTIFIER), new CToken(":")),
            new CTokenSequence(
                new CToken(CTokenType.CASE),
                new CTokenRepeat(valid_expression_tokens, false, 0, (tokens, startIndex, numMatched) => 
                {
                    int i, countQueries = 0, countColons = 0;

                    for (i = startIndex; i - startIndex < numMatched; i++)
                        if (tokens[i].tokenCode == "?")
                            countQueries++;
                        else if (tokens[i].tokenCode == ":")
                            countColons++;

                    if (countColons == countQueries + 1 && tokens[i - 1].tokenCode == ":")
                        return true;

                    return false;
                })
            ),
            new CTokenSequence(new CToken(CTokenType.DEFAULT), new CToken(":"))
        );

        public static readonly IMatchable selection_statement = new CTokenAlternative(
            new CTokenSequence(new CToken(CTokenType.IF), new CTokenEnclosed(new CToken("("), new CToken(")"))),
            new CToken(CTokenType.ELSE),
            new CTokenSequence(new CToken(CTokenType.SWITCH), new CTokenEnclosed(new CToken("("), new CToken(")")))
        );

        public static readonly IMatchable iteration_statement = new CTokenAlternative(
            new CToken(CTokenType.DO),
            new CTokenSequence(new CToken(CTokenType.WHILE), new CTokenEnclosed(new CToken("("), new CToken(")"))),
            new CTokenSequence(new CToken(CTokenType.FOR), new CTokenEnclosed(new CToken("("), new CToken(")")))
        );

        public static readonly IMatchable expression_statement = new CTokenSequence(expression_repeat, new CToken(";"));

        public static readonly IMatchable statement = new CTokenAlternative(
            labeled_statement,
            compound_statement,
            selection_statement,
            iteration_statement,
            jump_statement,
            
            expression_statement
        );


        public static IEnumerable<CMatchedData> matchExpression(IMatchable expression, 
            CToken[] tokens, bool getFirstMatches = true, int startIndex = 0)
        {
            int i;
            MatchResult currResult;

            for (i = startIndex; i < tokens.Length; i += getFirstMatches && currResult.isMatch ? currResult.numMatches : 1)
            {
                currResult = expression.match(tokens, i);

                if (currResult.isMatch && currResult.numMatches > 0)
                    yield return new CMatchedData(tokens, i, currResult.numMatches);
            }
        }

        public static IEnumerable<CFunctionDefinition> getFunctionContents(CToken[] tokens)
        {
            foreach (CMatchedData matched in matchExpression(function_declarator, tokens))
            {
                MatchResult currResult = compound_statement.match(tokens, matched.startIndex + matched.numMatches);

                if (currResult.isMatch && currResult.numMatches > 0)
                {
                    string functionName;
                    int i;

                    for (i = matched.startIndex; i < tokens.Length && tokens[i].tokenCode != "("; i++) ;

                    functionName = tokens[i - 1].tokenCode;

                    yield return new CFunctionDefinition(functionName,
                        new CParser().parseCompoundStatement(
                            new Queue<CMatchedData>(
                                new CMatchedData[] { 
                                    new CMatchedData(tokens, matched.startIndex + matched.numMatches, currResult.numMatches) 
                                }
                            )
                        )
                    );
                }
            }
        }

        public static IEnumerable<CMatchedData> matchCompoundStatementContents(CToken[] tokens)
        {
            if (tokens[0].tokenCode != "{" || tokens[tokens.Length - 1].tokenCode != "}")
                throw new System.ArgumentException("tokens should represent a compound statement!", "tokens");

            return matchExpression(statement, tokens, true, 1);
        }

        public static string getText(CToken[] tokens, int startIndex, int endIndex)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            int i;

            for (i = startIndex; i < endIndex; i++)
                builder.Append(tokens[i].tokenCode + " ");

            return builder.ToString().TrimEnd();
        }

        /* Labels */
        private List<CLabeledStatement> labels = new List<CLabeledStatement>();

        private List<CLabeledStatement> caseLabels = new List<CLabeledStatement>();

        /* Jumps */
        private List<CJumpStatement> returnJumps = new List<CJumpStatement>();

        private List<CJumpStatement> gotoJumps = new List<CJumpStatement>();

        private List<CJumpStatement> breakJumps = new List<CJumpStatement>();

        private List<CJumpStatement> continueJumps = new List<CJumpStatement>();


        public CLabeledStatement parseLabeledStatement(Queue<CMatchedData> matchedData)
        {
            CToken[] currTokens = matchedData.Dequeue().getTokens();
            string labelName = currTokens[0].tokenCode;
            CExpression caseExpression = null;

            CStatement labeled = parseStatement(matchedData);

            CLabeledStatement result;

            if (labelName == "case")
                caseExpression = new CExpression(getText(currTokens, 1, currTokens.Length - 1));

            if (labeled == null)
                throw new System.InvalidOperationException("error: label at end of compound statement!");

            result = new CLabeledStatement(labelName, caseExpression, labeled);

            if (labelName == "case" || labelName == "default")
                this.caseLabels.Add(result);
            else
            {
                int i;

                /* Match label with goto */
                for (i = 0; i < this.gotoJumps.Count; i++)
                    if (this.gotoJumps[i].targetIdentifier == labelName)
                    {
                        this.gotoJumps[i].setTargetStatement(result);
                        this.gotoJumps.RemoveAt(i);

                        return result;
                    }

                this.labels.Add(result);
            }

            return result;
        }

        public CCompoundStatement parseCompoundStatement(Queue<CMatchedData> matchedData)
        {
            List<CStatement> containedStatements =
                new List<CStatement>();

            CToken[] currTokens = matchedData.Dequeue().getTokens();

            Queue<CMatchedData> containedMatchedData =
                new Queue<CMatchedData>(matchCompoundStatementContents(currTokens));

            CStatement currStatement = null;

            containedStatements.Add(currStatement = parseStatement(containedMatchedData));

            while (currStatement != null)
                containedStatements.Add(currStatement = currStatement.nextStatement);

            return new CCompoundStatement(containedStatements.ToArray(), parseStatement(matchedData));
        }

        public CExpressionStatement parseExpressionStatement(Queue<CMatchedData> matchedData)
        {
            CToken[] currTokens = matchedData.Dequeue().getTokens();

            CExpression currExpression = new CExpression(getText(currTokens, 0, currTokens.Length - 1));

            return new CExpressionStatement(currExpression, parseStatement(matchedData));
        }

        public CConditionalStatement parseIfStatement(Queue<CMatchedData> matchedData)
        {
            CToken[] currTokens = matchedData.Dequeue().getTokens();

            CExpression conditionExpression;

            CStatement ifTrueStatement;

            if (currTokens[0].tokenType != CTokenType.IF)
                throw new System.ArgumentException("matchedData should start with an if statement!", "matchedData");

            conditionExpression = new CExpression(getText(currTokens, 2, currTokens.Length - 1));

            ifTrueStatement = parseStatement(matchedData);

            /* Check if next statement is ELSE */
            if (ifTrueStatement.nextStatement is CConditionalStatement &&
                ((CConditionalStatement)ifTrueStatement.nextStatement).condition == null)
            {
                CStatement elseStatement = ((CConditionalStatement)ifTrueStatement.nextStatement).ifTrueStatement;

                return new CConditionalStatement(conditionExpression, ifTrueStatement, elseStatement,
                    ifTrueStatement.nextStatement.nextStatement);
            }

            return new CConditionalStatement(conditionExpression, ifTrueStatement, ifTrueStatement.nextStatement);
        }

        public CConditionalStatement parseElseStatement(Queue<CMatchedData> matchedData)
        {
            CToken[] currTokens = matchedData.Dequeue().getTokens();

            CStatement ifTrueStatement;

            if (currTokens[0].tokenType != CTokenType.ELSE)
                throw new System.ArgumentException("matchedData should start with an else statement!", "matchedData");

            ifTrueStatement = parseStatement(matchedData);

            return new CConditionalStatement(null, ifTrueStatement, ifTrueStatement.nextStatement);
        }

        public CSwitchStatement parseSwitchStatement(Queue<CMatchedData> matchedData)
        {
            CToken[] currTokens = matchedData.Dequeue().getTokens();

            CExpression conditionExpression;

            CStatement switchStatement, nextStatement;

            CSwitchStatement result;

            List<CLabeledStatement> switchCases;

            int i;

            if (currTokens[0].tokenType != CTokenType.SWITCH)
                throw new System.ArgumentException("matchedData should start with a switch statement!", "matchedData");

            conditionExpression = new CExpression(getText(currTokens, 2, currTokens.Length - 1));

            switchCases = new List<CLabeledStatement>();

            switchStatement = parseStatement(matchedData);

            nextStatement = switchStatement.nextStatement;

            for (i = 0; i < this.caseLabels.Count; i++)
                if (switchStatement.contains(this.caseLabels[i]))
                {
                    switchCases.Add(this.caseLabels[i]);
                    this.caseLabels.RemoveAt(i);
                    i--;
                }

            result = new CSwitchStatement(conditionExpression, switchStatement,
                switchCases.ToArray(), nextStatement);

            for (i = 0; i < this.breakJumps.Count; i++)
                if (switchStatement.contains(this.breakJumps[i]))
                {
                    this.breakJumps[i].setTargetStatement(result.nextStatement as CLabeledStatement);
                    this.breakJumps.RemoveAt(i);
                    i--;
                }

            return result;
        }

        public CLabeledStatement parseWhileStatement(Queue<CMatchedData> matchedData)
        {
            CToken[] currTokens = matchedData.Dequeue().getTokens();

            CExpression conditionExpression;

            CStatement loopStatement, nextStatement;

            CLabeledStatement continueTarget, breakTarget;

            CJumpStatement continueLoop;

            int i;

            if (currTokens[0].tokenType != CTokenType.WHILE)
                throw new System.ArgumentException("matchedData should start with a while statement!", "matchedData");

            conditionExpression = new CExpression(getText(currTokens, 2, currTokens.Length - 1));

            loopStatement = parseStatement(matchedData);

            /* Create loop */

            nextStatement = loopStatement.nextStatement;

            breakTarget = new CLabeledStatement("while-loop: break target", null,
                nextStatement != null ? nextStatement : new CExpressionStatement(new CExpression(""), null));

            continueLoop = new CJumpStatement("continue", null);
            loopStatement.setNextStatement(continueLoop);

            continueTarget = new CLabeledStatement("while-loop: continue target", null, new CConditionalStatement(
                conditionExpression,
                new CCompoundStatement(new CStatement[] { loopStatement, continueLoop }, null),
                breakTarget
            ));

            continueLoop.setTargetStatement(continueTarget);

            for (i = 0; i < this.breakJumps.Count; i++)
                if (continueTarget.contains(this.breakJumps[i]))
                {
                    this.breakJumps[i].setTargetStatement(breakTarget);
                    this.breakJumps.RemoveAt(i);
                    i--;
                }

            for (i = 0; i < this.continueJumps.Count; i++)
                if (continueTarget.contains(this.continueJumps[i]))
                {
                    this.continueJumps[i].setTargetStatement(continueTarget);
                    this.continueJumps.RemoveAt(i);
                    i--;
                }

            return continueTarget;
        }

        public CCompoundStatement parseDoWhileStatement(Queue<CMatchedData> matchedData)
        {
            CToken[] currTokens = matchedData.Dequeue().getTokens();

            CStatement loopStatement, nextStatement;

            CLabeledStatement continueTarget, breakTarget;

            CJumpStatement continueLoop;

            int i;

            if (currTokens[0].tokenType != CTokenType.DO)
                throw new System.ArgumentException("matchedData should start with a do-while statement!", "matchedData");

            loopStatement = parseStatement(matchedData);

            if (loopStatement.nextStatement is CLabeledStatement && 
                loopStatement.nextStatement.nextStatement is CLabeledStatement)
                nextStatement = ((CLabeledStatement)loopStatement.nextStatement.nextStatement).labeledStatement;
            else
                throw new System.ArgumentException("incorrect do-while statement!", "matchedData");

            /* Create loop */

            breakTarget = new CLabeledStatement("do-while-loop: break target", null,
                nextStatement != null ? nextStatement : new CExpressionStatement(new CExpression(""), null));

            continueLoop = new CJumpStatement("continue", null);

            continueTarget = new CLabeledStatement("do-while-loop: continue target", null, loopStatement);

            continueLoop.setTargetStatement(continueTarget);

            loopStatement.setNextStatement(
                new CConditionalStatement(
                    ((CConditionalStatement)((CLabeledStatement)loopStatement.nextStatement).labeledStatement).condition,
                    continueLoop,
                    null
                )
            );

            for (i = 0; i < this.breakJumps.Count; i++)
                if (continueTarget.contains(this.breakJumps[i]))
                {
                    this.breakJumps[i].setTargetStatement(breakTarget);
                    this.breakJumps.RemoveAt(i);
                    i--;
                }

            for (i = 0; i < this.continueJumps.Count; i++)
                if (continueTarget.contains(this.continueJumps[i]))
                {
                    this.continueJumps[i].setTargetStatement(continueTarget);
                    this.continueJumps.RemoveAt(i);
                    i--;
                }

            return new CCompoundStatement(
                new CStatement[] { continueTarget, loopStatement.nextStatement },
                breakTarget
            );
        }

        public CCompoundStatement parseForStatement(Queue<CMatchedData> matchedData)
        {
            CToken[] currTokens = matchedData.Dequeue().getTokens();

            CExpression startExpression, conditionExpression, iterationExpression;

            CStatement loopStatement, nextStatement;

            CLabeledStatement continueTarget, breakTarget, iterationStart;

            CJumpStatement continueLoop;

            int i, j;

            if (currTokens[0].tokenType != CTokenType.FOR)
                throw new System.ArgumentException("matchedData should start with a for statement!", "matchedData");

            for (i = 0; i < currTokens.Length && currTokens[i].tokenCode != ";"; i++) ;
            startExpression = new CExpression(getText(currTokens, 2, i));

            for (j = i + 1; j < currTokens.Length && currTokens[j].tokenCode != ";"; j++) ;
            conditionExpression = new CExpression(getText(currTokens, i + 1, j));

            iterationExpression = new CExpression(getText(currTokens, j + 1, currTokens.Length - 1));

            loopStatement = parseStatement(matchedData);


            nextStatement = loopStatement.nextStatement;

            /* Create loop */

            nextStatement = loopStatement.nextStatement;

            breakTarget = new CLabeledStatement("for-loop: break target", null,
                nextStatement != null ? nextStatement : new CExpressionStatement(new CExpression(""), null));

            continueLoop = new CJumpStatement("for-loop: next iteration", null);

            continueTarget = new CLabeledStatement("for-loop: continue target", null,
                        new CExpressionStatement(iterationExpression, continueLoop));
            loopStatement.setNextStatement(continueTarget);

            iterationStart = new CLabeledStatement("for-loop: iteration start", null, new CConditionalStatement(
                conditionExpression,
                new CCompoundStatement(new CStatement[]
                {
                    loopStatement,
                    continueTarget,
                    continueLoop
                }, null),
                null
            ));

            continueLoop.setTargetStatement(iterationStart);

            for (i = 0; i < this.breakJumps.Count; i++)
                if (loopStatement.contains(this.breakJumps[i]))
                {
                    this.breakJumps[i].setTargetStatement(breakTarget);
                    this.breakJumps.RemoveAt(i);
                    i--;
                }

            for (i = 0; i < this.continueJumps.Count; i++)
                if (loopStatement.contains(this.continueJumps[i]))
                {
                    this.continueJumps[i].setTargetStatement(continueTarget);
                    this.continueJumps.RemoveAt(i);
                    i--;
                }

            return new CCompoundStatement(
                new CStatement[]
                {
                    new CExpressionStatement(startExpression, iterationStart),
                    iterationStart
                },
                breakTarget);
        }

        public CJumpStatement parseJumpStatement(Queue<CMatchedData> matchedData)
        {
            CToken[] currTokens = matchedData.Dequeue().getTokens();

            CJumpStatement result;

            if (currTokens[0].tokenType == CTokenType.RETURN)
            {
                if (currTokens.Length > 1)
                {
                    CExpression returnExpression = new CExpression(getText(currTokens, 1, currTokens.Length - 1));

                    result = new CJumpStatement(returnExpression, parseStatement(matchedData));
                }
                else
                    result = new CJumpStatement("return", parseStatement(matchedData));

                this.returnJumps.Add(result);
            }
            else
            {
                result = new CJumpStatement(getText(currTokens, 0, currTokens.Length - 1), parseStatement(matchedData));

                if (currTokens[0].tokenType == CTokenType.GOTO)
                {
                    int i;

                    /* Match goto with label */
                    for (i = 0; i < this.labels.Count; i++)
                        if (this.labels[i].codeString == result.targetIdentifier)
                        {
                            result.setTargetStatement(this.labels[i]);

                            return result;
                        }

                    this.gotoJumps.Add(result);
                }
                else if (currTokens[0].tokenType == CTokenType.BREAK)
                    this.breakJumps.Add(result);
                else if (currTokens[0].tokenType == CTokenType.CONTINUE)
                    this.continueJumps.Add(result);
                else
                    throw new System.ArgumentException("matchedData does not represent a valid jump statement!", "matchedData");
            }

            return result;
        }

        public CStatement parseStatement(Queue<CMatchedData> matchedData)
        {
            CMatchedData currMatched;

            if (matchedData.Count == 0)
                return null;

            currMatched = matchedData.Peek();

            switch (currMatched[0].tokenType)
            {
                case CTokenType.IF:
                    return parseIfStatement(matchedData);

                case CTokenType.ELSE:
                    return parseElseStatement(matchedData);

                case CTokenType.SWITCH:
                    return parseSwitchStatement(matchedData);

                case CTokenType.WHILE:
                    return parseWhileStatement(matchedData);

                case CTokenType.DO:
                    return parseDoWhileStatement(matchedData);

                case CTokenType.FOR:
                    return parseForStatement(matchedData);

                case CTokenType.GOTO:
                case CTokenType.RETURN:
                case CTokenType.CONTINUE:
                case CTokenType.BREAK:
                    return parseJumpStatement(matchedData);

                case CTokenType.LEFT_BRACE:
                    return parseCompoundStatement(matchedData);
            }

            if (currMatched[currMatched.numMatches - 1].tokenCode == ":")
                return parseLabeledStatement(matchedData);

            return parseExpressionStatement(matchedData);
        }
    }
}
