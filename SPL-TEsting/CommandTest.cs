﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AssignmentASE;
using System.Text.RegularExpressions;
using System.Linq;

namespace SPL_Testing
{
    /// <summary>
    /// Tests for the parser class and command factory 
    /// </summary>
    [TestClass]
    public class CommandTest
    {
        Painter p;

        /// <summary>
        /// Tests the moveto method
        /// Tests if the cursor has moved to x and y
        /// Tests the parsecommand method
        /// Tests the painter class
        /// </summary>
        [TestMethod]
        public void TestParseCommandMoveTo()
        {
            p = new Painter();
            Parser parser = new Parser(p);
            parser.parseCommand("moveto 50,50", 0);
            Assert.AreEqual(50, p.xPos);
            Assert.AreEqual(50, p.yPos);
        }
        /// <summary>
        /// Tests the drawto method
        /// Tests if cursor has drawn to x y
        /// Tests the parsecommand method
        /// Tests the painter class
        /// </summary>
        [TestMethod]
        public void TestParseCommandDrawTo()
        {
            p = new Painter();
            Parser parser = new Parser(p);
            parser.parseCommand("drawto 120,90", 0);
            Assert.AreEqual(120, p.xPos);
            Assert.AreEqual(90, p.yPos);
        }

        /// <summary>
        /// Tests the reset method
        /// Tests whether the cursor is reset on top left
        /// Tests the parsecommand method
        /// Tests the move to method
        /// Tests the painter class
        /// </summary>
        [TestMethod]
        public void TestParseCommandReset()
        {
            p = new Painter();
            Parser parser = new Parser(p);
            parser.parseCommand("moveto 80,50", 0);
            Assert.AreEqual(80, p.xPos);
            parser.parseCommand("reset", 0);
            Assert.AreEqual(0, p.xPos);
            Assert.AreEqual(0, p.yPos);
        }

        /// <summary>
        /// Tests the pen method
        /// Tests if the pen color changed
        /// Tests the parsecommand method
        /// Tests the painter class
        /// </summary>
        [TestMethod]
        public void TestParseCommandColor()
        {
            p = new Painter();
            Parser parser = new Parser(p);
            parser.parseCommand("pen blue", 0);
            Assert.AreEqual("blue", p.Pen.ToLower());
        }

        /// <summary>
        /// Tests the fill method
        /// Tests if fill is on
        /// Tests the parsecommand method
        /// Tests the painter class
        /// </summary>
        [TestMethod]
        public void TestParseCommandFillOn()
        {
            p = new Painter();
            Parser parser = new Parser(p);
            parser.parseCommand("fill on", 0);
            Assert.IsTrue(p.fill);
        }

        /// <summary>
        /// Tests the fill method
        /// Tests if the fill is off
        /// Tests the parsecommand method
        /// Tests the painter class
        /// </summary>
        [TestMethod]
        public void TestParseCommandFillOff()
        {
            p = new Painter();
            Parser parser = new Parser(p);
            parser.parseCommand("fill off", 0);
            Assert.IsFalse(p.fill);
        }

        /// <summary>
        /// Tests the variable method
        /// Tests the parsecommand method
        /// Tests the painter class
        /// Checks if the value has been assigned to variable
        /// </summary>
        [TestMethod]
        public void TestParseCommandVariable()
        {
            p = new Painter();
            Parser parser = new Parser(p);
            parser.parseCommand("var x = 5", 0);
            Assert.IsTrue(5 == parser.Variables["x"]);
        }

        /// <summary>
        /// Tests the whileLoop method
        /// Tests the parsecommand method
        /// Tests the painter class
        /// Checks if the while condition has been evaluated
        /// </summary>
        [TestMethod]
        public void TestParseCommandWhile()
        {
            p = new Painter();
            Parser parser = new Parser(p);
            parser.parseCommand("var x = 5", 0);
            parser.parseCommand("while x < 20", 0);
            Assert.IsTrue(20 > parser.Variables["x"]);
        }

        [TestMethod]
        public void TestLexerVariable()
        {
            //var splitOnAssignment = new Regex(@"[\s=]", RegexOptions.Compiled);
            //var splitOnOperators = new Regex(@"[-+\\\*]", RegexOptions.Compiled);

            //var expression = "num = 1 + 2";

            //var output = splitOnAssignment.Split(expression).Where(token => token != String.Empty).ToArray<string>();

            //var variable = new Variable();

            //Console.WriteLine(output[0]);
            //foreach(string x in splitOnOperators.Split(output[1]).Where(token => token != String.Empty).ToArray<string>())
            //    Console.WriteLine(x);

            

        }
    }
}
