from typing import Optional
from fastapi import FastAPI
from fastapi.responses import HTMLResponse

app = FastAPI()
import uvicorn
from graphene import ObjectType, String, Schema

import grpc
import verification_pb2
import verification_pb2_grpc



def IsValidNumber(number: str, count: int):
    if number.isdigit() and len(number) == count:
        return True
    else:
        return False


class Query(ObjectType):
    status = String(phone=String(default_value="000000"), pin=String(default_value="0000"))

    def resolve_status(self, info, phone, pin):
        # 89017890224 1234
        print(f"{phone} {pin}")
        if IsValidNumber(phone, 11) and IsValidNumber(pin, 4):
            # open a gRPC channel
            channel = grpc.insecure_channel('localhost:5001')

            # create a stub (client)
            stub = verification_pb2_grpc.VerificationStub(channel)

            # create a valid request message
            number = verification_pb2.PhonePinRequest(phone=int(phone), pin=int(1234))

            # make the call
            response = stub.CheckClient(number)

            print(response.status)
            return response.status
        else:
            return False


schema = Schema(query=Query)


@app.get("/verif")
def verif(data: str):
    print("data: ", data)  # Print  { status (phone: "89017890224", pin: "1234")
    result = schema.execute(data)  # execute query  data in method resolve_status
    return str(result)


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)

    # http://127.0.0.1:8000/verif?data=%7B+status+%28phone%3A+%2289017890224%22%2C+pin%3A+%221234%22%29%7D
#     http://127.0.0.1:8000/verif?data=%7Bstatus%28phone%3A%2289017890224%22%2Cpin%3A%221234%22%29%7D

