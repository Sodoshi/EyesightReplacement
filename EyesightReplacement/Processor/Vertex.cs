namespace EyesightReplacement.Processor
{
    public class Vertex
    {
        private readonly float[] _normalData = new float[3];
        private readonly float[] _positionData = new float[3];
        private readonly float[] _textureData = new float[2];
        public Vertex(float posX, float posY, float posZ, float normX, float normY, float normZ, float texX, float texY)
        {
            _positionData[0] = posX;
            _positionData[1] = posY;
            _positionData[2] = posZ;
            _normalData[0] = normX;
            _normalData[1] = normY;
            _normalData[2] = normZ;
            _textureData[0] = texX;
            _textureData[1] = texY;
        }
        public float[] NormalData()
        {
            return _normalData;
        }
        public float[] PositionData()
        {
            return _positionData;
        }
        public float[] TextureData()
        {
            return _textureData;
        }
    }
}