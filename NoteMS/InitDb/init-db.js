db = db.getSiblingDB('dm_note');

db.createCollection('Notes', {
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: ['Id', 'PatientId', 'Patient', 'PatientNote'],
            properties: {
                Id: {
                    bsonType: 'int',
                    description: 'must be a int and is required'
                },
                PatientId: {
                    bsonType: 'string',
                    description: 'must be a string and is required'
                },
                Patient: {
                    bsonType: 'string',
                    description: 'must be a string and is required'
                },
                PatientNote: {
                    bsonType: 'string',
                    description: 'must be a string and is required'
                }
            }
        }
    }
});
