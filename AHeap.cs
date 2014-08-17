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

public class AHeap
{
	private List<ANode> mNodes = new List<ANode>();
	public List<ANode> Nodes
	{
		get { return mNodes; }
	}

	public ANode Root
	{
		get
		{
			if( mNodes.Count > 0 )
				return mNodes[0];
			else
				return null;
		}
	}

	public int Count
	{
		get { return mNodes.Count; }
	}

	public void Clear()
	{
		mNodes.Clear();
	}

	public ANode PopFirst()
	{
		if( mNodes.Count > 0 )
		{
			ANode node = mNodes[ 0 ];
			mNodes.RemoveAt( 0 );
			return node;
		}
		
		return null;
	}

	public ANode PopLast()
	{
		if( mNodes.Count > 0 )
		{
			ANode node = mNodes[ mNodes.Count - 1 ];
			mNodes.RemoveAt( mNodes.Count - 1 );
			return node;
		}

		return null;
	}

	public void Add( ANode _node )
	{
		if( ! mNodes.Contains( _node ) )
		{
			mNodes.Add( _node );
		}
	}

	public void Remove( ANode _node )
	{
		if( mNodes.Contains( _node ) )
		{
			mNodes.Remove( _node );
		}
	}

	public bool Contains( ANode _node )
	{
		return mNodes.Contains( _node );
	}

	public void Sort()
	{
		mNodes.Sort();
	}
}