using Domain.Entities;
using Xunit;

namespace TestProject1
{
    public class TableTests
    {
        // CONSTRUCTOR
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_WithInvalidNumber_Throws(int num)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            // Utiliza el método constructor de la clase
                new Table( 
                    num,// Number
                    4 // Capacity
                    ));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_WithInvalidCapacity_Throws(int capacity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            // Utiliza el método constructor de la clase
                new Table(
                    1,// Number
                    capacity // Capacity
                    ));
        }
        [Theory]
        [InlineData(2, 4)]
        public void Constructor_WithValidArguments(int num, int capacity)
        {
           
            // Utiliza el método constructor de la clase
            var table = new Table( num, capacity );
            
            Assert.Equal(num, table.Number);
            Assert.Equal(capacity, table.Capacity);
            Assert.True(table.Status);

        }

        // OTROS MÉTODOS
        
        [Fact]
        public void Occupy_WhenTableIsAvailable_ChangesStatusToOccupied()
        {
            var table = new Table(3, 4);

            table.Occupy();

            Assert.False(table.Status);
        }

        [Fact]
        public void Occupy_WhenTableIsOccupied_Throws()
        {
            var table = new Table(3, 4);

            table.Occupy();

            Assert.Throws<InvalidOperationException>(() =>
                table.Occupy()
            );
        }

        [Fact]
        public void Release_WhenTableIsOccupied_ChangesStatusToAvailable()
        {
            var table = new Table(3, 4);
            table.Occupy(); // Porque por default el status es true 

            table.Release();

            Assert.True(table.Status);

        }

        [Fact]
        public void Release_WhenTableIsOccupied_Throws()
        {
            var table = new Table(3, 4);

            Assert.Throws<InvalidOperationException>(() =>
                table.Release()
            );
        }


    }
}
