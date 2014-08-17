/*
- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
- AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
- THE SOFTWARE.
*/

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class ANode : IComparable<ANode>
{
	public const int HorzVertCost = 10;
	public const int DiagCost = 14;

	private ANode mParent = null;
	public ANode Parent
	{
		get { return mParent; }
		set { mParent = value; }
	}

	private int mx = 0;
	public int x
	{
		get { return mx; }
		set { mx = value; }
	}
	
	private int mz = 0;
	public int z
	{
		get { return mz; }
		set { mz = value; }
	}
	
	public int f
	{
		get { return g + h; }
	}

	private int mg = 0;
	public int g
	{
		get { return mg; }
		set { mg = value; }
	}

	private int mh = 0;
	public int h
	{
		get { return mh; }
		set { mh = value; }
	}

	private bool mWalkable = true;
	public bool Walkable
	{
		get { return mWalkable; }
		set { mWalkable = value; }
	}

	private UDirection mDirection = UDirection.None;
	public UDirection Direction
	{
		get { return mDirection; }
		set { mDirection = value; }
	}

	public int CompareTo( ANode _node )
	{
		return f.CompareTo( _node.f );
	}

	public override string ToString ()
	{
		return "x: " + x + ":" + z;
	}
}

public enum UDirection
{
	None,
	Cross,
	Diagonal
}