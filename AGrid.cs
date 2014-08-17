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

public class AGrid
{
	private ANode[,] mNodes;

	private int mWidth = 10;
	public int Width
	{
		get { return mWidth; }
	}
	private int mDepth = 10;
	public int Depth
	{
		get { return mDepth; }
	}

	private float mNodeSize = 1.0f;
	public float NodeSize
	{
		get { return mNodeSize; }
	}

	private bool mIncludeDiagonal = true;
	public bool IncludeDiagonal
	{
		get { return mIncludeDiagonal; }
		set { mIncludeDiagonal = value; }
	}

	private AHeap mOpenHeap = new AHeap();
	private AHeap mClosedHeap = new AHeap();

	private ANode mStartNode = null;
	public ANode StartNode
	{
		get { return mStartNode; }
	}

	private ANode mEndNode = null;
	public ANode EndNode
	{
		get { return mEndNode; }
	}

	private List<Vector3> mPath = new List<Vector3>();
	public List<Vector3> Path
	{
		get { return mPath; }
	}

	public AGrid( int _width, int _depth, float _nodeSize )
	{
		mWidth = _width;
		mDepth = _depth;
		mNodeSize = _nodeSize;

		mNodes = new ANode[ mDepth, mWidth ];

		Setup();
	}

	public void Setup()
	{
		for( int z = 0; z < mDepth; z++ )
		{
			for( int x = 0; x < mWidth; x++ )
			{	
				ANode node = new ANode();

				node.z = z;
				node.x = x;

				mNodes[ z, x ] = node;
			}
		}
	}

	public Vector3 GridToWorldPosition( int _x, int _z )
	{
		return new Vector3( _x * mNodeSize + ( mNodeSize / 2 ), 0, _z * mNodeSize + ( mNodeSize / 2 ) );
	}

	public Vector3 WorldPositionToGrid( Vector3 _position )
	{
		return new Vector3( Mathf.FloorToInt( _position.x / mNodeSize ), 0, Mathf.FloorToInt( _position.z / mNodeSize ) );
	}

	public bool IsSolid( int _x, int _z )
	{
		Vector3 currentPos = GridToWorldPosition( _x, _z );

		RaycastHit hit;
		
		if( Physics.Raycast( currentPos, -Vector3.up, out hit, Mathf.Infinity ) )
		{
			if( hit.collider.gameObject.tag == "Ground" )
			{
				return true;
			}
		}

		return false;
	}

	public bool IsValidPosition( int _x, int _z )
	{
		if( _x < 0 || _x >= mWidth )
			return false;

		if( _z < 0 || _z >= mDepth )
			return false;

		return true;
	}

	public void FindPath( Vector3 _start, Vector3 _end )
	{
		Vector3 start = WorldPositionToGrid( _start );
		Vector3 end = WorldPositionToGrid( _end );

		FindPath( (int)start.x, (int)start.z, (int)end.x, (int)end.z );
	}

	public void FindPath( int _xStart, int _zStart, int _xEnd, int _zEnd )
	{
		// Clear all lists
		mOpenHeap.Clear();
		mClosedHeap.Clear();
		mPath.Clear();

		// If position is not valid... do nothing
		if( ! IsValidPosition( _xStart, _zStart ) || ! IsValidPosition( _xEnd, _zEnd ) )
			return;

		// If start == end... do nothing
		if( _xStart == _xEnd && _zStart == _zEnd )
			return;

		mStartNode = FindNode( _xStart, _zStart );
		mEndNode = FindNode( _xEnd, _zEnd );

		mOpenHeap.Add( mStartNode );
		mClosedHeap.Add( mStartNode );

		mStartNode.g = 0;
		mStartNode.h = HCost( _xStart, _zStart, _xEnd, _zEnd );

		while( mOpenHeap.Count > 0 )
		{
			mOpenHeap.Sort();

			//Remove and use first anode from OpenHeap
			ANode currentNode = mOpenHeap.PopFirst();

			if( currentNode == mEndNode )
				break;

			// Add currentNode to ClosedHeap because it was already visited
			if( ! mClosedHeap.Contains( currentNode ) )
				mClosedHeap.Add( currentNode );

			// Find all the neighbors of the currentNode
			List<ANode> neighbors = GetConnectedNodes( currentNode );
			foreach( ANode neighbor in neighbors )
			{
				// If neighbor has already been visited... ignore it
				if( mClosedHeap.Contains( neighbor ) )
					continue;

				// parent's (currentNode) gscore plus GCost
				int gTentative = currentNode.g + GCost( neighbor );

				// If NOT in OpenHeap OR the GScore hasn't increased
				if( ! mOpenHeap.Contains( neighbor ) || gTentative < neighbor.g )
				{
					neighbor.Parent = currentNode;
					neighbor.g = gTentative;
					neighbor.h = HCost( neighbor.x, neighbor.z, mEndNode.x, mEndNode.z );

					if( ! mOpenHeap.Contains( neighbor ) )
						mOpenHeap.Add( neighbor );
				}
			}
		}


		// IF mEndNode doesn't have a parent than no path was found
		if( mEndNode.Parent != null )
			SetPath();
	}

	public List<ANode> GetConnectedNodes( ANode _node )
	{
		List<ANode> nodes = new List<ANode>();
		ANode node;

		node = FindNode( _node.x + 1, _node.z );
		if( node != null && node.Walkable )
		{
			node.Direction = UDirection.Cross;
			nodes.Add( node );
		}

		node = FindNode( _node.x, _node.z + 1 );
		if( node != null && node.Walkable )
		{
			node.Direction = UDirection.Cross;
			nodes.Add( node );
		}

		node = FindNode( _node.x - 1, _node.z );
		if( node != null && node.Walkable )
		{
			node.Direction = UDirection.Cross;
			nodes.Add( node );
		}
		
		node = FindNode( _node.x, _node.z - 1 );
		if( node != null && node.Walkable )
		{
			node.Direction = UDirection.Cross;
			nodes.Add( node );
		}

		// Only include diagonal neighbors if IncludeDiagonal is true
		if( mIncludeDiagonal )
		{
			node = FindNode( _node.x + 1, _node.z + 1 );
			if( node != null && node.Walkable )
			{
				node.Direction = UDirection.Diagonal;
				nodes.Add( node );
			}
			
			node = FindNode( _node.x - 1, _node.z + 1 );
			if( node != null && node.Walkable )
			{
				node.Direction = UDirection.Diagonal;
				nodes.Add( node );
			}
			
			node = FindNode( _node.x - 1, _node.z - 1 );
			if( node != null && node.Walkable )
			{
				node.Direction = UDirection.Diagonal;
				nodes.Add( node );
			}
			
			node = FindNode( _node.x + 1, _node.z - 1 );
			if( node != null && node.Walkable )
			{
				node.Direction = UDirection.Diagonal;
				nodes.Add( node );
			}
		}

		return nodes;
	}

	public ANode FindNode( int _x, int _z )
	{
		if( IsValidPosition( _x, _z ) )
			return mNodes[ _z, _x ];

		return null;
	}

	private int GCost( ANode _node )
	{
		if( _node.Direction == UDirection.Diagonal )
			return 15;

		return 10;
	}

	private int HCost( int _xStart, int _zStart, int _xEnd, int _zEnd )
	{
		int x = (int) ( Math.Abs( _xStart - _xEnd ) );
		int z = (int) ( Math.Abs( _zStart - _zEnd ) );

		return ( x + z ) * 10;
	}

	public void SetPath()
	{
		mPath.Clear();

		Path.Add( GridToWorldPosition( mEndNode.x, mEndNode.z ) );

		ANode current = mEndNode;
		
		while( current != mStartNode )
		{
			if( current.Parent != null )
			{
				mPath.Add( GridToWorldPosition( current.x, current.z ) );
			}
			
			current = current.Parent;
		}
		
		if( current != null )
		{
			mPath.Add( GridToWorldPosition( current.x, current.z ) );
		}
	}
}